using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    [Header("Post-Wave")]
    [SerializeField] private float lootGraceDuration = 5f;

    [Header("Waves")]
    [SerializeField] private int totalWaves = 10;
    [SerializeField] private int baseEnemies = 3;              // enemies in wave 1
    [SerializeField] private int enemiesPerWaveIncrease = 2;   // added each wave

    [Header("Enemy Prefabs")]
    [Tooltip("Normal enemy types that can appear in regular waves (1..totalWaves-1).")]
    [SerializeField] private GameObject[] normalEnemyPrefabs;

    [Tooltip("Boss enemy used ONLY in the final wave.")]
    [SerializeField] private GameObject bossPrefab;

    [Header("Spawning")]
    [SerializeField] private Transform[] spawnPoints;          // assign spawn positions
    [SerializeField] private float timeBetweenEnemySpawns = 0.5f;
    [SerializeField] private float timeBetweenWaves = 3f;

    [Header("Next Scene (after boss dies)")]
    [Tooltip("Name of the scene to load when all waves are complete. Leave empty to do nothing.")]
    [SerializeField] private string nextSceneName = "";
    [Tooltip("Alternative: build index of the scene to load. Used only if nextSceneName is empty and index >= 0.")]
    [SerializeField] private int nextSceneBuildIndex = -1;

    [Header("Boss Intro")]
    [SerializeField] private float bossIntroCameraDuration = 5f;


    private int currentWave = 0;
    private int enemiesAlive = 0;          // total (keep if you want)
    private int enemiesAliveThisWave = 0;
    private bool allWavesCompleted = false;

    private void OnEnable()
    {
        Enemy.OnAnyEnemyDied += HandleEnemyDied;
    }

    private void OnDisable()
    {
        Enemy.OnAnyEnemyDied -= HandleEnemyDied;
    }

    private void Awake()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            // gather all direct children as spawn points
            var list = new System.Collections.Generic.List<Transform>();
            foreach (Transform child in transform)
                list.Add(child);

            spawnPoints = list.ToArray();
        }
    }


    private void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            Debug.LogWarning("[WaveManager] No spawn points assigned.");

        if (normalEnemyPrefabs == null || normalEnemyPrefabs.Length == 0)
            Debug.LogWarning("[WaveManager] No normal enemy prefabs assigned.");

        StartCoroutine(RunWaves());
    }


    private void HandleEnemyDied(Enemy e)
    {
        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);
        enemiesAliveThisWave = Mathf.Max(0, enemiesAliveThisWave - 1);

        //Debug.Log($"[WaveManager] Enemy died: {e.name} ({e.GetInstanceID()}) " +
        //         $"Global={enemiesAlive}, Wave={enemiesAliveThisWave}");
    }

    private IEnumerator RunWaves()
    {
        while (currentWave < totalWaves)
        {
            currentWave++;
            // Debug.Log($"[WaveManager] Starting wave {currentWave}/{totalWaves}");

            // reset per-wave counter BEFORE spawning
            enemiesAliveThisWave = 0;

            yield return StartCoroutine(SpawnWave(currentWave));

            // wait until all enemies spawned in THIS wave are dead
            yield return new WaitUntil(() => enemiesAliveThisWave == 0);

            if (currentWave < totalWaves)
            {
               // Debug.Log($"[WaveManager] Wave {currentWave} cleared. Next wave in {timeBetweenWaves} seconds.");
                yield return new WaitForSeconds(timeBetweenWaves);
            }
            else
            {
               // Debug.Log("[WaveManager] All waves complete! Boss defeated.");
                allWavesCompleted = true;
                HandleAllWavesCompleted();
            }
        }
    }

    private IEnumerator SpawnWave(int waveNumber)
    {
        if (waveNumber < totalWaves)
        {
            int count = GetEnemyCountForWave(waveNumber);
            for (int i = 0; i < count; i++)
            {
                SpawnRandomNormalEnemy();
                yield return new WaitForSeconds(timeBetweenEnemySpawns);
            }
        }
        else
        {
            // Final wave: spawn ONLY boss
            if (bossPrefab != null)
            {
                Debug.Log("[WaveManager] Final wave: spawning boss only.");
                GameObject bossInstance = Spawn(bossPrefab);

                if (bossInstance != null)
                    StartCoroutine(BossIntroSequence(bossInstance.transform));
            }
            else
            {
                Debug.LogWarning("[WaveManager] Boss prefab is missing. Spawning normal enemies instead.");
                int count = GetEnemyCountForWave(waveNumber);
                for (int i = 0; i < count; i++)
                {
                    SpawnRandomNormalEnemy();
                    yield return new WaitForSeconds(timeBetweenEnemySpawns);
                }
            }
        }
    }

    private IEnumerator BossIntroSequence(Transform bossTransform)
    {
        if (bossTransform == null)
            yield break;

        // Find camera follow
        var cam = FindFirstObjectByType<CameraFollow>();
        if (cam == null)
            yield break;

        // Cache player to focus back later
        var player = FindFirstObjectByType<Player>();
        Transform playerT = player ? player.transform : null;

        // 1) Freeze gameplay (player, boss, projectiles, physics, etc.)
        float oldTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        // 2) Switch to boss music
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBossMusic();

        // 3) Focus camera on boss
        cam.SetTarget(bossTransform);

        // 4) Wait in real time (unaffected by timeScale)
        if (bossIntroCameraDuration > 0f)
            yield return new WaitForSecondsRealtime(bossIntroCameraDuration);

        // 5) Return camera to player
        if (playerT != null)
            cam.SetTarget(playerT);

        // 6) Restore map BGM
        if (AudioManager.Instance != null)
        {
            var sceneName = SceneManager.GetActiveScene().name;

            if (sceneName == "ForestMap")
                AudioManager.Instance.PlayForestMusic();
            else if (sceneName == "DesertMap")
                AudioManager.Instance.PlayDesertMusic();
            else
                AudioManager.Instance.PlayMainMenuMusic(); // fallback
        }

        // 7) Unfreeze gameplay
        Time.timeScale = oldTimeScale;
    }

    private int GetEnemyCountForWave(int waveNumber)
    {
        // Wave 1: baseEnemies
        // Wave 2: baseEnemies + enemiesPerWaveIncrease
        // Wave 3: baseEnemies + 2 * enemiesPerWaveIncrease, etc.
        return baseEnemies + (waveNumber - 1) * enemiesPerWaveIncrease;
    }

    private void SpawnRandomNormalEnemy()
    {
        if (normalEnemyPrefabs == null || normalEnemyPrefabs.Length == 0)
        {
            Debug.LogWarning("[WaveManager] No normalEnemyPrefabs assigned, cannot spawn.");
            return;
        }

        var prefab = normalEnemyPrefabs[Random.Range(0, normalEnemyPrefabs.Length)];
        Spawn(prefab);
    }

    private GameObject Spawn(GameObject prefab)
    {
        if (!prefab)
        {
            Debug.LogWarning("[WaveManager] Missing prefab in Spawn().");
            return null;
        }
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("[WaveManager] No spawn points to spawn from.");
            return null;
        }

        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject instance = Instantiate(prefab, sp.position, Quaternion.identity);

        enemiesAlive++;
        enemiesAliveThisWave++;   // track this wave only

        return instance;
    }

    private void HandleAllWavesCompleted()
    {
        // run as coroutine so we can WaitForSeconds
        if (!gameObject.activeInHierarchy)
            return;

        StartCoroutine(HandleAllWavesCompletedCo());
    }


    private IEnumerator HandleAllWavesCompletedCo()
    {
        Time.timeScale = 1f;

        var currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"[WaveManager] All waves complete in scene: {currentScene}");


        if (currentScene == "ForestMap") // first map
        {
            if (lootGraceDuration > 0f)
            {
                Debug.Log($"[WaveManager] Loot grace period: {lootGraceDuration} seconds.");
                yield return new WaitForSeconds(lootGraceDuration);
            }

            MapManager.CompleteForestMap();   // mark forest as done
            SceneManager.LoadScene("WorldMap"); // back to map selection
            yield break;
        }

        if (currentScene == "DesertMap") // second map
        {
            // If loot grace on the last map
            /*
            if (lootGraceDuration > 0f)
            {
                Debug.Log($"[WaveManager] Final-map loot grace: {lootGraceDuration} seconds.");
                yield return new WaitForSeconds(lootGraceDuration);
            }
            */

            MapManager.CompleteDesertMap(); // track completion
            SceneManager.LoadScene("VictoryMenu");
            yield break;
        }

        // Fallback / other maps
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            if (lootGraceDuration > 0f)
                yield return new WaitForSeconds(lootGraceDuration);

            SceneManager.LoadScene(nextSceneName);
        }
        else if (nextSceneBuildIndex >= 0)
        {
            if (lootGraceDuration > 0f)
                yield return new WaitForSeconds(lootGraceDuration);

            SceneManager.LoadScene(nextSceneBuildIndex);
        }
        else
        {
            Debug.Log("[WaveManager] All waves complete, but no next scene configured.");
        }
    }
}
using UnityEngine;

public class PlayerSpawnerManager : MonoBehaviour
{
    [Header("Player Prefabs")]
    [SerializeField] private GameObject charmanderPrefab;
    [SerializeField] private GameObject squirtlePrefab;
    [SerializeField] private GameObject pikachuPrefab;

    [Header("Spawn Point")]
    [SerializeField] private Transform spawnPoint;

    private void Awake()
    {
        if (spawnPoint == null)
            spawnPoint = transform;  // fallback: use this object as spawn point

        // 1) If a Player already exists (persistent from previous scene), just move & reset it
        Player existingPlayer = FindFirstObjectByType<Player>();
        if (existingPlayer != null)
        {
            existingPlayer.transform.position = spawnPoint.position;

            // FULL revive + state reset (gets you out of faint)
            existingPlayer.ResetForRetry();

            // re-register buffs link in RunManager
            var buffs = existingPlayer.GetComponent<Entity_Buffs>();
            if (RunManager.Instance != null && buffs != null)
                RunManager.Instance.RegisterPlayer(buffs);

            return;
        }

        // 2) No player yet: spawn the selected prefab
        string selected = PlayerPrefs.GetString("SelectedPokemon", "Charmander");
        GameObject prefab = GetPrefabByName(selected);
        if (!prefab)
        {
            Debug.LogError($"[PlayerSpawner] No prefab for '{selected}'. Falling back to Charmander.");
            prefab = charmanderPrefab;
        }

        GameObject playerInstance = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        // Make this Player persistent across maps
        DontDestroyOnLoad(playerInstance);

        // register buffs for this new Player
        var newBuffs = playerInstance.GetComponent<Entity_Buffs>();
        if (RunManager.Instance != null && newBuffs != null)
            RunManager.Instance.RegisterPlayer(newBuffs);
    }

    private GameObject GetPrefabByName(string name)
    {
        switch (name)
        {
            case "Charmander": return charmanderPrefab;
            case "Squirtle": return squirtlePrefab;
            case "Pikachu": return pikachuPrefab;
            default: return null;
        }
    }
}
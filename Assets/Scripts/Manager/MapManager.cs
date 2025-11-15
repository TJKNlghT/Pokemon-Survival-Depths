using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    [Header("Map Markers")]
    public GameObject ForestMap;
    public GameObject DesertMap;

    [Header("Preview Anchors")]
    [SerializeField] private Transform forestPreviewAnchor;
    [SerializeField] private Transform desertPreviewAnchor;

    [Header("Preview Prefabs")]
    [SerializeField] private GameObject charmanderPreviewPrefab;
    [SerializeField] private GameObject squirtlePreviewPrefab;
    [SerializeField] private GameObject pikachuPreviewPrefab;

    // which map is “armed” but not yet entered
    private int pendingSceneIndex = -1;
    private GameObject currentPreview;

    void Start()
    {
        UpdateMapMarkers();

        // Ensure world map music plays whenever we enter this scene
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMainMenuMusic();   // PlayWorldMapMusic() -- need a world map music maybe exploration
        }
    }

    void Update()
    {
        // Back to previous scene (e.g. character select)
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            HandleBackToSelection();
        }
    }

    private void HandleBackToSelection()
    {
        PlaySelectSound();

        bool forestCompleted = PlayerPrefs.GetInt("ForestCompleted", 0) == 1;
        bool desertCompleted = PlayerPrefs.GetInt("DesertCompleted", 0) == 1;
        bool anyMapCompleted = forestCompleted || desertCompleted;

        // If no maps completed yet -> just go back and let player change Pokémon
        if (!anyMapCompleted)
        {
            SceneManager.LoadScene(3); // your Pokémon selection scene index
            return;
        }

        // If some map(s) already completed -> treat as "new run"
        var player = FindFirstObjectByType<Player>();
        if (player != null)
            Destroy(player.gameObject);

        if (RunManager.Instance != null)
            RunManager.Instance.ResetRun();

        // IMPORTANT: we do NOT clear ForestCompleted/DesertCompleted here
        // so map unlock progress is kept unless you want a totally fresh profile.
        // If you want full reset, you could add:
        // PlayerPrefs.DeleteKey("ForestCompleted");
        // PlayerPrefs.DeleteKey("DesertCompleted");
        // PlayerPrefs.DeleteKey("SelectedPokemon");

        SceneManager.LoadScene(3);
    }

    void UpdateMapMarkers()
    {
        bool forestCompleted = PlayerPrefs.GetInt("ForestCompleted", 0) == 1;

        // Forest:
        //  - visible only if NOT completed
        if (ForestMap != null)
            ForestMap.SetActive(!forestCompleted);

        // Desert:
        //  - visible only AFTER forest is completed
        if (DesertMap != null)
            DesertMap.SetActive(forestCompleted);

    }

    // UI: click on Forest marker
    public void OnForestMapClick()
    {
        // safety: if forest is completed, ignore clicks
        bool forestCompleted = PlayerPrefs.GetInt("ForestCompleted", 0) == 1;
        if (forestCompleted)
        {
            Debug.Log("[MapManager] Forest already completed; cannot re-enter.");
            PlaySelectSound(); // or play error sound
            return;
        }

        pendingSceneIndex = 5; // Forest scene build index
        SaveCurrentMap(pendingSceneIndex);
        PlaySelectSound();

        SpawnPokemonPreview(forestPreviewAnchor); // your preview function

        StartCoroutine(LoadMapAfterDelay(pendingSceneIndex, 1.5f));
    }

    // UI: click on Desert marker
    public void OnDesertMapClick()
    {
        pendingSceneIndex = 6;               // Desert scene
        SaveCurrentMap(pendingSceneIndex);
        PlaySelectSound();

        SpawnPokemonPreview(desertPreviewAnchor);

        StartCoroutine(LoadMapAfterDelay(pendingSceneIndex, 1.5f)); // 1.5 seconds preview
    }

    private System.Collections.IEnumerator LoadMapAfterDelay(int sceneIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadMap(sceneIndex);
    }

    void LoadMap(int sceneIndex)
    {
        // optional: play BGM for the map you’re about to enter
        PlayMapBGM(sceneIndex);

        // actually go to the map NOW
        SceneManager.LoadScene(sceneIndex);
    }

    void SaveCurrentMap(int sceneIndex)
    {
        PlayerPrefs.SetInt("CurrentMapScene", sceneIndex);
        PlayerPrefs.Save();
    }

    void PlayMapBGM(int sceneIndex)
    {
        if (AudioManager.Instance == null)
            return;

        switch (sceneIndex)
        {
            case 5:
                AudioManager.Instance.PlayForestMusic();
                break;
            case 6:
                AudioManager.Instance.PlayDesertMusic();
                break;
        }
    }

    void PlaySelectSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySelectSound();
    }

    private void SpawnPokemonPreview(Transform anchor)
    {
        if (anchor == null)
        {
            Debug.LogWarning("[MapManager] Preview anchor is null.");
            return;
        }

        if (currentPreview != null)
            Destroy(currentPreview);

        string selected = PlayerPrefs.GetString("SelectedPokemon", "Charmander");
        GameObject prefab = GetPreviewPrefabByName(selected);

        if (prefab == null)
        {
            Debug.LogWarning($"[MapManager] No preview prefab for '{selected}'.");
            return;
        }

        //instantiate AS CHILD of the anchor (inside the Canvas)
        currentPreview = Instantiate(prefab, anchor);
        currentPreview.transform.localPosition = Vector3.zero;
    }

    private GameObject GetPreviewPrefabByName(string name)
    {
        switch (name)
        {
            case "Charmander": return charmanderPreviewPrefab;
            case "Squirtle": return squirtlePreviewPrefab;
            case "Pikachu": return pikachuPreviewPrefab;
            default: return charmanderPreviewPrefab;
        }
    }

    // Existing static completion methods...
    public static void CompleteForestMap()
    {
        PlayerPrefs.SetInt("ForestCompleted", 1);
        PlayerPrefs.Save();
    }

    public static void CompleteDesertMap()
    {
        PlayerPrefs.SetInt("DesertCompleted", 1);
        PlayerPrefs.Save();
    }
}
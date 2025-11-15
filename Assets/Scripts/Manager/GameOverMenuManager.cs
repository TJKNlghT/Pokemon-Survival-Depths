using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    void Start()
    {
        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameOverMusic();
        }
    }

    // Called by UI button
    public void OnTryAgain()
    {
        // 1) figure out which map to reload
        string lastMap = RunManager.Instance != null
            ? RunManager.Instance.GetLastMapSceneName()
            : "ForestScene"; // fallback

        // 2) clear buffs that were gained in this map attempt
        if (RunManager.Instance != null)
            RunManager.Instance.ClearCurrentMapBuffs();

        // 3) just reload the map; a NEW player will be spawned there
        SceneManager.LoadScene(lastMap);
    }


    // Called by UI button
    public void OnBackToMenu()
    {
        // Kill the persistent player so next run is fresh
        var player = FindFirstObjectByType<Player>();
        if (player != null)
            Destroy(player.gameObject);

        // Reset run data (buff sources, last map, etc.)
        RunManager.Instance?.ResetRun();

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
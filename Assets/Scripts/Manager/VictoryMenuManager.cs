using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu"; // set in Inspector if needed

    private void Start()
    {
        // Play victory music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayVictoryMusic();
        }

        // Make sure timeScale is normal
        Time.timeScale = 1f;

        // Destroy the persistent player so a new run starts clean
        var player = FindFirstObjectByType<Player>();
        if (player != null)
            Destroy(player.gameObject);

        // Reset run data (buff sources, last map, etc.)
        RunManager.Instance?.ResetRun();

        // Optional: clear selected pokemon for next run
        PlayerPrefs.DeleteKey("SelectedPokemon");
    }

    // Hook this to a "Main Menu" button
    public void OnReturnToMenu()
    {
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {

            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogWarning("[VictoryManager] mainMenuSceneName not set.");
        }
    }

    // Hook this to a "Quit" button
    public void OnQuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
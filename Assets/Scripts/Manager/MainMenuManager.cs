using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private void Start()
    {
        // Optional: make sure main menu music is playing
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMainMenuMusic();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // NEW GAME: wipe run + map progress
            if (RunManager.Instance != null)
            {
                RunManager.Instance.ResetAllProgress();
            }
            else
            {
                // Fallback if RunManager hasn't spawned yet
                PlayerPrefs.DeleteKey("ForestCompleted");
                PlayerPrefs.DeleteKey("DesertCompleted");
                PlayerPrefs.DeleteKey("CurrentMapScene");
                PlayerPrefs.Save();
            }

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySelectSound();

            SceneManager.LoadScene(1);   // your Player Selection scene
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsMenu2Manager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            PlaySelectSound();
            SceneManager.LoadScene(8);
        }
    }

    void PlaySelectSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySelectSound();
        }
        else
        {
            Debug.LogWarning("AudioManager Instance is null!");
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsButtonOnClick : MonoBehaviour
{  
    public void OnSettingsButton()
    {
        PlaySelectSound();
        SceneManager.LoadScene(2);
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
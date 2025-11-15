using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonOnClick : MonoBehaviour
{
    public void OnPlayButton()
    {
        PlaySelectSound();
        SceneManager.LoadScene(3);
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
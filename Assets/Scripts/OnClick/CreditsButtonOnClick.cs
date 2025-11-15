using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsButtonOnClick : MonoBehaviour
{

    public void OnCreditsButton()
    {
        PlaySelectSound();
        SceneManager.LoadScene(8);
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
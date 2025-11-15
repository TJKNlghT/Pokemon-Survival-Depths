using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonOnClick : MonoBehaviour
{
    public void OnBackButton()
    {
        PlaySelectSound();
        SceneManager.LoadScene(4);
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
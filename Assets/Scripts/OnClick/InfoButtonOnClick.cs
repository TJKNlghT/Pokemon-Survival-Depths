using UnityEngine;
using UnityEngine.SceneManagement;

public class InfoButtonOnClick : MonoBehaviour
{
    public void OnInfoButton()
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
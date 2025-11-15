using UnityEngine;
using UnityEngine.SceneManagement;

public class NextButtonOnClick : MonoBehaviour
{

    public void OnNextButton()
    {
        PlaySelectSound();
        SceneManager.LoadScene(9);
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
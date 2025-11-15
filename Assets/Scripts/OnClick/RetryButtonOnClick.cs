using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryButtonOnClick : MonoBehaviour
{
    public void OnRetryButton()
    {
        PlaySelectSound();

        int savedSceneIndex = PlayerPrefs.GetInt("CurrentMapScene", 5);

        SceneManager.LoadScene(savedSceneIndex);
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
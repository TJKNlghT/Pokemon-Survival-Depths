using UnityEngine;
using UnityEngine.SceneManagement;

public class NextButton_2_OnClick : MonoBehaviour
{

    public void OnNextButton()
    {
        PlaySelectSound();
        SceneManager.LoadScene(12);
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
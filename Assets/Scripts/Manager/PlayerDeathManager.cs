using UnityEngine;

using UnityEngine.SceneManagement;

public class PlayerDeathManager: MonoBehaviour
{
    [SerializeField] private string gameOverSceneName = "GameOverMenu";

    private void OnEnable()
    {
        Player.OnPlayerFaint += HandlePlayerFaint;
    }

    private void OnDisable()
    {
        Player.OnPlayerFaint -= HandlePlayerFaint;
    }

    private void HandlePlayerFaint()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameOverSceneName);
    }
}

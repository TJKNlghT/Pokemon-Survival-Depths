using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySelectionMenuManager : MonoBehaviour
{
    [Header("Character Panels")]
    public GameObject CharmanderPanel;
    public GameObject SquirtlePanel;
    public GameObject PikachuPanel;

    private string selectedPokemon;

    void Start()
    {
        HideAllPanels();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            PlaySelectSound();
            SceneManager.LoadScene(1);
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

    public void OnCharmanderButton()
    {
        ShowCharmanderPanel();
        PlaySelectSound();
    }
    
    public void OnSquirtleButton()
    {
        ShowSquirtlePanel();
        PlaySelectSound();
    }
    
    public void OnPikachuButton()
    {
        ShowPikachuPanel();
        PlaySelectSound();
    }

    public void OnCharmanderStartButton()
    {
        selectedPokemon = "Charmander";
        SaveSelectedPokemon();
        StartGame();
    }
    
    public void OnSquirtleStartButton()
    {
        selectedPokemon = "Squirtle";
        SaveSelectedPokemon();
        StartGame();
    }
    
    public void OnPikachuStartButton()
    {
        selectedPokemon = "Pikachu";
        SaveSelectedPokemon();
        StartGame();
    }

    void StartGame()
    {
        PlaySelectSound();
        SceneManager.LoadScene(4);
    }

      void ShowCharmanderPanel()
    {
        HideAllPanels();
        if (CharmanderPanel != null)
            CharmanderPanel.SetActive(true);
    }
    
    void ShowSquirtlePanel()
    {
        HideAllPanels();
        if (SquirtlePanel != null)
            SquirtlePanel.SetActive(true);
    }
    
    void ShowPikachuPanel()
    {
        HideAllPanels();
        if (PikachuPanel != null)
            PikachuPanel.SetActive(true);
    }
    
    void HideAllPanels()
    {
        if (CharmanderPanel != null) CharmanderPanel.SetActive(false);
        if (SquirtlePanel != null) SquirtlePanel.SetActive(false);
        if (PikachuPanel != null) PikachuPanel.SetActive(false);
    }

    void SaveSelectedPokemon()
    {
        PlayerPrefs.SetString("SelectedPokemon", selectedPokemon);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class ForestMapManager : MonoBehaviour
{
    [Header("Character Icons")]
    public GameObject CharmanderIcon;
    public GameObject SquirtleIcon;
    public GameObject PikachuIcon;

    void Start()
    {   
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayForestMusic();
        }

        ShowSelectedPokemonIcon();

        // tell RunManager we just started this map
        RunManager.Instance?.BeginMap(SceneManager.GetActiveScene().name); // Remember need to at to other map managers

        // Capture XP/level snapshot for this map
        var player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            var xp = player.GetComponent<Player_XP>();
            xp?.CaptureMapEntrySnapshot();
        }
    }

    void ShowSelectedPokemonIcon()
    {
        HideAllIcons();
        
        string selectedPokemon = PlayerPrefs.GetString("SelectedPokemon", "");
        
        switch (selectedPokemon)
        {
            case "Charmander":
                if (CharmanderIcon != null) 
                    CharmanderIcon.SetActive(true);
                break;
            case "Squirtle":
                if (SquirtleIcon != null) 
                    SquirtleIcon.SetActive(true);
                break;
            case "Pikachu":
                if (PikachuIcon != null) 
                    PikachuIcon.SetActive(true);
                break;
            default:
                Debug.LogWarning("No Pokemon selected or invalid selection: " + selectedPokemon);
                break;
        }
    }

    void HideAllIcons()
    {
        if (CharmanderIcon != null) CharmanderIcon.SetActive(false);
        if (SquirtleIcon != null) SquirtleIcon.SetActive(false);
        if (PikachuIcon != null) PikachuIcon.SetActive(false);
    }
}
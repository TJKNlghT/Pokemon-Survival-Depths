using UnityEngine;

public class DesertMapManager : MonoBehaviour
{
    [Header("Character Icons")]
    public GameObject CharmanderIcon;
    public GameObject SquirtleIcon;
    public GameObject PikachuIcon;

    [Header("Player Characters")]
    public GameObject Player_Charmander;
    public GameObject Player_Squirtle;
    public GameObject Player_Pikachu;

    void Start()
    {   
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDesertMusic();
        }

        ShowSelectedPokemonIcon();
        ActivateSelectedPlayer();
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

    void ActivateSelectedPlayer()
    {
        if (Player_Charmander != null) Player_Charmander.SetActive(false);
        if (Player_Squirtle != null) Player_Squirtle.SetActive(false);
        if (Player_Pikachu != null) Player_Pikachu.SetActive(false);
        
        string selectedPokemon = PlayerPrefs.GetString("SelectedPokemon", "");
        
        switch (selectedPokemon)
        {
            case "Charmander":
                if (Player_Charmander != null) 
                    Player_Charmander.SetActive(true);
                break;
            case "Squirtle":
                if (Player_Squirtle != null) 
                    Player_Squirtle.SetActive(true);
                break;
            case "Pikachu":
                if (Player_Pikachu != null) 
                    Player_Pikachu.SetActive(true);
                break;
            default:
                Debug.LogWarning("No Pokemon selected, defaulting to Charmander");
                if (Player_Charmander != null) 
                    Player_Charmander.SetActive(true);
                break;
        }
    }
}
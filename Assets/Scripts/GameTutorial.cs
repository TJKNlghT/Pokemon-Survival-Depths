using UnityEngine;
using System.Collections;

public class GameTutorial : MonoBehaviour
{
    [Header("Tutorial UI")]
    public GameObject tutorialPanel;
    
    private bool isTutorialActive = false;
    
    void Start()
    {
        Invoke("StartTutorial", 0f);
    }
    
    void StartTutorial()
    {
        isTutorialActive = true;
        Time.timeScale = 0f;
        tutorialPanel.SetActive(true);
    }
    
    void Update()
    {
        if (!isTutorialActive) return;
        
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || 
            Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {
            EndTutorial();
        }
    }
    
    void EndTutorial()
    {
        isTutorialActive = false;
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
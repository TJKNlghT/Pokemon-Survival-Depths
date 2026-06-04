using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Serializable]
    public class TutorialStep
    {
        public GameObject panelRoot;
        public bool closeOnMovementKeys = false;
        public bool closeOnAnyKey = true;
    }

    [Header("Steps in order")]
    [SerializeField] private List<TutorialStep> steps = new List<TutorialStep>();

    private int currentStepIndex = -1;
    private bool isRunning = false;

    private void Start()
    {
        HideAllPanels();

        if (steps == null || steps.Count == 0)
            return;

        // only show once per run, using RunManager
        if (RunManager.Instance != null)
        {
            // if it returns false, intro tutorial was already shown this run -> skip entirely
            if (!RunManager.Instance.ConsumeIntroTutorialFlag())
                return;
        }

        BeginTutorial();
    }

    private void Update()
    {
        if (!isRunning || currentStepIndex < 0 || currentStepIndex >= steps.Count)
            return;

        var step = steps[currentStepIndex];

        if (step.closeOnMovementKeys)
        {
            if (Input.GetKeyDown(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.A) ||
                Input.GetKeyDown(KeyCode.S) ||
                Input.GetKeyDown(KeyCode.D))
            {
                AudioManager.Instance.PlaySelectSound();
                NextStep();
            }
        }
        else if (step.closeOnAnyKey)
        {
            if (Input.anyKeyDown)
            {
                AudioManager.Instance.PlaySelectSound();
                NextStep();
            }
        }
    }

    private void BeginTutorial()
    {
        isRunning = true;
        Time.timeScale = 0f;
        currentStepIndex = 0;
        ShowCurrentStep();
    }

    private void NextStep()
    {
        currentStepIndex++;

        if (currentStepIndex >= steps.Count)
            EndTutorial();
        else
            ShowCurrentStep();
    }

    private void EndTutorial()
    {
        HideAllPanels();
        isRunning = false;
        Time.timeScale = 1f;
    }

    private void ShowCurrentStep()
    {
        HideAllPanels();

        var step = steps[currentStepIndex];
        if (step != null && step.panelRoot != null)
            step.panelRoot.SetActive(true);
    }

    private void HideAllPanels()
    {
        if (steps == null) return;
        foreach (var s in steps)
        {
            if (s != null && s.panelRoot != null)
                s.panelRoot.SetActive(false);
        }
    }

    public void SkipCurrentStepViaButton()
    {
        if (!isRunning) return;
        NextStep();
    }
}
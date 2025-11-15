using UnityEngine;

public class UI : MonoBehaviour
{
    public UI_InfoBox infoBox { get; private set; }

    private void Awake()
    {
        // Finds even if InfoBox GameObject starts inactive
        infoBox = GetComponentInChildren<UI_InfoBox>(true);
        if (!infoBox)
            Debug.LogError("UI_InfoBox not found under this UI. Make sure InfoBox is a child of the same Canvas/parent.");
    }
}
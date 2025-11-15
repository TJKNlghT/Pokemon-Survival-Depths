using TMPro;
using UnityEngine;

public class UI_InfoBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infoText;
    public void Set(string text)
    {
        if (infoText) infoText.text = text;
    }

    public void Clear()
    {
        if (infoText) infoText.text = "Please Select One Upgrade";
    }
}

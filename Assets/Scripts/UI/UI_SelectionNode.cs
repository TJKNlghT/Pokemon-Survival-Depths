using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SelectionNode : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI ui;

    [Header("Optional (design-time)")]
    [SerializeField] private Buff_DataSO buffData;   // only used if Bind(...) not called
    [SerializeField] private Image buffIcon;
    [SerializeField] private string selectionColorHex = "#E3E3E3";

    private Color lastColor;
    private Buff_DataSO data;                         // runtime-bound choice
    private System.Action<Buff_DataSO> onPick;

    private void Awake()
    {
        ui = GetComponentInParent<UI>(true);

        if (!buffIcon)
            buffIcon = GetComponentInChildren<Image>(true); // includeInactive = true

        if (!buffIcon)
            Debug.LogWarning($"[UI_SelectionNode] No Image found for buffIcon on {name}");
    }

    // Called by the panel when showing options
    public void Bind(Buff_DataSO d, System.Action<Buff_DataSO> pickCb)
    {
        data = d;
        onPick = pickCb;

        string buffName = d ? d.buffName : "NULL_BUFF";
        string iconName = (d && d.icon) ? d.icon.name : "NULL_ICON";
        string iconField = buffIcon ? buffIcon.name : "NULL_BUFFICON_REF";

        // Debug.Log($"[SelectionNode {name}] Binding buff = {buffName}, data.icon = {iconName}, buffIcon field = {iconField}");

        if (buffIcon && d && d.icon)
            buffIcon.sprite = d.icon;

        gameObject.name = d ? $"UI_SelectionNode - {d.name}" : "UI_SelectionNode";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPick?.Invoke(data ?? buffData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var chosen = data ?? buffData;            // prefer runtime-bound, fall back to inspector
        if (ui && ui.infoBox && chosen)
            ui.infoBox.Set($"<b>{chosen.buffName}</b>\n{chosen.description}");

        TintIcon(selectionColorHex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ui && ui.infoBox) ui.infoBox.Clear();
        RestoreIconTint();
    }

    private void TintIcon(string hex)
    {
        if (!buffIcon) return;
        lastColor = buffIcon.color;
        if (ColorUtility.TryParseHtmlString(hex, out var c))
            buffIcon.color = c;
    }

    private void RestoreIconTint()
    {
        if (buffIcon) buffIcon.color = lastColor;
    }
}
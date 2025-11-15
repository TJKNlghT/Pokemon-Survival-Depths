using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_LevelUpPanel : MonoBehaviour
{
    [Header("Wiring")]
    [SerializeField] private GameObject root;           // panel root (enable/disable)
    [SerializeField] private UI_SelectionNode[] nodes;  // exactly 3 buttons/cards

    [Header("Lifecycle")]
    [SerializeField] private bool destroyOnClose = true;

    private Entity_Buffs targetBuffs;

    public event Action Closed; 

    void Awake()
    {
        // If you forget to assign, default to parent as root
        if (!root) root = transform.parent ? transform.parent.gameObject : gameObject;

        if (nodes == null || nodes.Length == 0)
            nodes = GetComponentsInChildren<UI_SelectionNode>(true);

        root.SetActive(false);
    }

    public void Show(Buff_DataSO[] options, Entity_Buffs buffsTarget)
    {
        targetBuffs = buffsTarget;

        for (int i = 0; i < nodes.Length; i++)
        {
            bool has = options != null && i < options.Length && options[i] != null;
            nodes[i].gameObject.SetActive(has);
            if (has) nodes[i].Bind(options[i], OnPick);
        }

        root.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Hide()
    {
        root.SetActive(false);
        Time.timeScale = 1f;

        Closed?.Invoke();

        if (destroyOnClose && root) //destroy the whole prefab instance
            Destroy(root);
    }

    private void OnPick(Buff_DataSO picked)
    {
        if (targetBuffs == null || picked == null)
        {
            Hide();
            return;
        }

        // Entity_Buffs.Apply now returns a source ID for mapScoped/timed buffs
        string sourceId = targetBuffs.Apply(picked);

        if (picked.mapScoped && RunManager.Instance != null)
        {
            RunManager.Instance.RegisterMapBuffSource(sourceId);
        }

        Hide();
    }
}
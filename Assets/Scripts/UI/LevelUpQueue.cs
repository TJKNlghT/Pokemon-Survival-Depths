using System.Collections.Generic;
using UnityEngine;

public class LevelUpQueue : MonoBehaviour
{
    [Header("Prefab/Parents")]
    [SerializeField] private GameObject levelUpUIRootPrefab;   // prefab whose root is LevelUpUIRoot
    [SerializeField] private Canvas canvas;                    // parent canvas for spawned prefab

    private readonly Queue<(Buff_DataSO[] options, Entity_Buffs target)> _queue = new();
    private bool _isShowing;

    void Awake()
    {
        if (!canvas) canvas = FindFirstObjectByType<Canvas>();
    }

    public void Enqueue(Buff_DataSO[] options, Entity_Buffs target)
    {
        _queue.Enqueue((options, target));
        TryShowNext();
    }

    private void TryShowNext()
    {
        if (_isShowing || _queue.Count == 0) return;

        var (opts, tgt) = _queue.Dequeue();

        var uiRoot = Instantiate(levelUpUIRootPrefab, canvas.transform, false);
        var panel = uiRoot.GetComponentInChildren<UI_LevelUpPanel>(true);

        _isShowing = true;
        panel.Closed += () =>
        {
            _isShowing = false;
            TryShowNext();     // show next queued level-up
        };

        panel.Show(opts, tgt);
    }
}

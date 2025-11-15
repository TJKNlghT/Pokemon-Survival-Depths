using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon Setup / Buff Data", fileName = "Buff Data - ")]
public class Buff_DataSO : ScriptableObject
{
    [Header("Skill description")]
    public string buffName;
    [TextArea]
    public string description;
    public Sprite icon;

    [Serializable] public struct Buff { public StatType statType; public float value; }
    public List<Buff> buffs = new();

    [Header("Timed (optional)")]
    public bool isTimed = false;
    public float duration = 10f;

    [Header("Scope")]
    public bool mapScoped = true;

    [Header("Damage Scale")]
    public DamageScaleData damageScale;
}

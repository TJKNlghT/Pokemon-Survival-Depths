using System;
using UnityEngine;

[Serializable]
public class DamageScaleData
{
    [Header("Damage")]
    public float physical = 1;

    [Header("Damage")]
    public float slowDuration = 3;
    public float slowMultiplier = 0.2f;

    [Header("Burn")]
    public float burnDuration = 3;
    public float burnDamageScale = 1;

    [Header("Shock")]
    public float shockDuration = 3;
    public float shockDamageScale = 1.5f;
    public float shockCharge = 0.5f;
}

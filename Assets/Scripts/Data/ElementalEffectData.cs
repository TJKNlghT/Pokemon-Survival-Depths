using System;
using UnityEngine;

public class ElementalEffectData
{
    public float slowDuration;
    public float slowMultiplier;

    public float burnDuration;
    public float burnDamage;

    public float shockDuration;
    public float shockDamage;
    public float shockCharge;

    public ElementalEffectData(Entity_Stats entityStats, DamageScaleData damageScale)
    {
        slowDuration = damageScale.slowDuration;
        slowMultiplier = damageScale.slowMultiplier;

        burnDuration = damageScale.burnDuration;
        burnDamage = damageScale.burnDamageScale;

        shockDuration = damageScale.shockDuration;
        shockDamage = damageScale.shockDamageScale;
        shockCharge = damageScale.shockCharge;

    }
}



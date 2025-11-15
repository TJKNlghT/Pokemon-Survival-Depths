using System;
using UnityEngine;

[Serializable]
public class AttackData
{
    public float physicalDamage;
    public ElementalEffectData effectData;

    public AttackData(Entity_Stats entityStats, DamageScaleData scaleData)
    {
        physicalDamage = entityStats.GetPhysicalDamage(scaleData.physical);
        
        effectData = new ElementalEffectData(entityStats, scaleData);
    }
}

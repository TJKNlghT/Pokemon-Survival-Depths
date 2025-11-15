using System;
using UnityEngine;

[Serializable]
public class Stat_Regen
{
    // extra regen per tick (additive)
    public Stat bonusHealthRegen;

    public Stat regenCooldownReduction; // percentage
}
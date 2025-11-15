using System;
using System.Collections.Generic;
using UnityEngine;

public class PickUps_PermanantBuffExample : PickUps
{
    [Header("Buff")]
    [SerializeField] private string buffId; // or name
    [SerializeField] private List<Buff> buffs = new();

    protected override bool TryApplyPickup(Player player)
    {
            if (player == null || player.stats == null) return false;

            foreach (var buff in buffs)
            {
                var stat = player.stats.GetStatByType(buff.statType);
                if (stat != null) 
                    stat.AddModifier(buff.value, buffId);
            }

            // If max health changed, avoid over-health
            var hp = player.GetComponent<Entity_Health>();
            hp?.ClampToMax(); // implement ClampToMax() if you don’t have it yet

            return true; // consumed.
    }
}

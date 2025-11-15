using System;
using System.Collections.Generic;
using UnityEngine;

public class TimedPickUps_TemporaryBuffExample : TimedPickUps
{
    [SerializeField] private string buffId;
    [SerializeField] private List<Buff> buffs = new();


    // unique source name used in Stat.AddModifier(..., source)
    private string runtimeSourceId;

    protected override bool CanApplyTo(Player player)
    {
        // return false here if you want to disallow duplicates
        //checks here
        return player && player.stats;
    }

    protected override void OnBuffStart(Player player)
    {
        var stats = player.stats;
        if (stats == null) return;

        // make a unique source for THIS pickup instance
        runtimeSourceId = $"{buffId}#{Guid.NewGuid():N}";

        foreach (var buff in buffs)
        {
            var stat = stats.GetStatByType(buff.statType);
            if (stat != null)
                stat.AddModifier(buff.value, runtimeSourceId);
        }

        player.GetComponent<Entity_Health>()?.ClampToMax();
    }

    protected override void OnBuffEnd(Player player)
    {
        var stats = player.stats;
        if (stats == null || string.IsNullOrEmpty(runtimeSourceId)) return;

        foreach (var buff in buffs)
        {
            var stat = stats.GetStatByType(buff.statType);
            if (stat != null)
                stat.RemoveModifier(runtimeSourceId, RemoveMode.All);
        }

        player.GetComponent<Entity_Health>()?.ClampToMax();
    }
}
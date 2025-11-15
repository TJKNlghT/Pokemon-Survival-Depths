using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class Entity_Buffs : MonoBehaviour
{
    private Entity entity;
    private Entity_Stats stats;
    private Entity_Health health;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        stats = GetComponent<Entity_Stats>();
        health = GetComponent<Entity_Health>();
    }

    public string Apply(Buff_DataSO data)
    {
        if (!data || stats == null) return null;

        if (data.isTimed)
        {
            // unique source so overlapping instances expire independently
            string runtimeSource = $"{data.buffName}#T#{Guid.NewGuid():N}";
            StartCoroutine(ApplyTimedCo(data, runtimeSource));
            return runtimeSource;
        }
        else if (data.mapScoped)
        {
            // map-scoped permanent buff: we still give it a unique source
            string runtimeSource = $"{data.buffName}#M#{Guid.NewGuid():N}";

            foreach (var e in data.buffs)
            {
                var st = stats.GetStatByType(e.statType);
                if (st != null) st.AddModifier(e.value, runtimeSource);
            }

            health?.ClampToMax();
            return runtimeSource; // important: we can later remove only THESE buffs
        }
        else
        {
            // fully run-permanent buff (shared source name)
            foreach (var e in data.buffs)
            {
                var st = stats.GetStatByType(e.statType);
                if (st != null) st.AddModifier(e.value, data.buffName);
            }
            health?.ClampToMax();
            return null;
        }
    }

    public void RemoveBySource(string sourceId)
    {
        if (string.IsNullOrEmpty(sourceId) || stats == null) return;
        // We don’t know which stats were touched; safest to iterate all StatTypes you use.
        foreach (StatType t in Enum.GetValues(typeof(StatType)))
        {
            var st = stats.GetStatByType(t);
            if (st != null) st.RemoveModifier(sourceId, RemoveMode.All);
        }
        health?.ClampToMax();
    }

    private IEnumerator ApplyTimedCo(Buff_DataSO data, string runtimeSource)
    {
        // add
        foreach (var e in data.buffs)
        {
            var st = stats.GetStatByType(e.statType);
            if (st != null) st.AddModifier(e.value, runtimeSource);
        }
        health?.ClampToMax();

        // use realtime so it still counts down while Time.timeScale = 0 (level-up screen)
        yield return new WaitForSecondsRealtime(data.duration);

        // remove only this instance
        foreach (var e in data.buffs)
        {
            var st = stats.GetStatByType(e.statType);
            if (st != null) st.RemoveModifier(runtimeSource, RemoveMode.All);
        }
        health?.ClampToMax();
    }
}

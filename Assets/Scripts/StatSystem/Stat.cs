using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public enum RemoveMode
{
    First,
    All
}

[Serializable]
public class Stat
{
    [SerializeField] private float baseValue;
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();

    private bool needToBeReCalculated = true;
    private float finalValue;

    public float GetValue() 
    {
        if(needToBeReCalculated)
        {
            finalValue = GetFinalValue();
            needToBeReCalculated = false;
        }

        return finalValue;
    }

    private float GetFinalValue()
    {
        float finalValue = baseValue;

        foreach(var modifier in modifiers)
        {
            finalValue = finalValue + modifier.value;
        }
        return finalValue;
    }

    public void SetBaseValue(float value) => baseValue = value;

    public void AddModifier(float value, string source)
    {
        modifiers.Add(new StatModifier(value, source));
        needToBeReCalculated = true;

    }

    public int RemoveModifier(string source, RemoveMode mode = RemoveMode.All, float? value = null) // (source, mode, optional filter by value)
    {
        Predicate<StatModifier> match = m =>
            string.Equals(m.source, source, StringComparison.Ordinal) &&
            (!value.HasValue || Mathf.Approximately(m.value, value.Value));

        if (mode == RemoveMode.All)
        {
            int result = modifiers.RemoveAll(match);
            needToBeReCalculated = true;
            return result;
        }
        else // First
        {
            int idx = modifiers.FindIndex(match);
            if (idx >= 0)
            {
                modifiers.RemoveAt(idx);
                needToBeReCalculated = true;
                return 1;
            }
            return 0;
        }
    }

    // Remove by exact modifier (source + value), First/All.
    // Returns how many were removed.
    public int RemoveModifier(StatModifier toRemove, RemoveMode mode = RemoveMode.First)
    {
        if (toRemove == null) return 0;

        if (mode == RemoveMode.All)
        {
            int result = modifiers.RemoveAll(m => m.Equals(toRemove));
            needToBeReCalculated = true;
            return result;
        }
        else // First
        {
            int idx = modifiers.FindIndex(m => m.Equals(toRemove));
            if (idx >= 0)
            {
                modifiers.RemoveAt(idx);
                needToBeReCalculated = true;
                return 1;
            }
            return 0;
        }
    }
}

[Serializable]
public class StatModifier : IEquatable<StatModifier>
{
    public float value;
    public string source; // item pickups or selection

    public StatModifier(float value, string source)
    {
        this.value = value;
        this.source = source;
    }

    public bool Equals(StatModifier other)
    {
        if (other == null) return false;
        return Mathf.Approximately(value, other.value) &&
               string.Equals(source, other.source, StringComparison.Ordinal);
    }

    public override bool Equals(object obj) => Equals(obj as StatModifier);

    public override int GetHashCode()
    {
        unchecked
        {
            int h = 17;
            h = h * 23 + value.GetHashCode();
            h = h * 23 + (source?.GetHashCode() ?? 0);
            return h;
        }
    }
}

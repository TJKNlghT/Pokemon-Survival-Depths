using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon Setup / Default Stat", fileName = "Default Stat - ")]
public class Stat_SetupSO :ScriptableObject
{
    [Header("Resources")]
    public float maxHealth = 100;
    public float healthRegen;

    [Header("Offense")]
    public float attackSpeed = 1;
    public float damage = 10;
    public float critChance;
}

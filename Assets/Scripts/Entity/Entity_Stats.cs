using System.Text.RegularExpressions;
using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stat_SetupSO defaultStatSetup;


    public Stat_ResourceGroup resources;
    public Stat_MajorGroup major;
    public Stat_OffenseGroup offense;
    public Stat_DefenseGroup defense;
    public Stat_Bonus bonus;
    public Stat_Regen regen;

    public AttackData GetAttackData(DamageScaleData scaleData)
    {
        return new AttackData(this, scaleData);
    }

    [Header("Element")]
    [SerializeField] private ElementType elementType = ElementType.Normal;
    public ElementType Element => elementType;

    public float GetPhysicalDamage(float scaleFactor = 1)
    {
        float baseDamage = offense.damage.GetValue();
        float bonusDamage = major.strength.GetValue(); // strength +1 upgrade gives +1 damage.
        float totalBaseDamage = baseDamage + bonusDamage;

        float baseCritChance = offense.critChance.GetValue();
        float bonusCritChance = bonus.additionalCritChance.GetValue() * 2f; // additional crit chance is +2% per upgrade
        float critChance = baseCritChance + bonusCritChance;

        float baseCritPower = offense.critPower.GetValue();
        float bonusCritPower = bonus.additionalCritPower.GetValue() * .5f; //additional crit power is +0.5% per upgrade
        float critPower = (baseCritPower + bonusCritPower) / 100; // total crit power is a multiplier

        bool isCrit = Random.Range(0, 100) < critChance;
        float finalDamage = isCrit ? totalBaseDamage * critPower : totalBaseDamage;

        return finalDamage * scaleFactor;
    }

    public float GetArmorMitigation(float armorReduction)
    {
        float baseArmor = defense.armor.GetValue();
        float bonusArmor = bonus.additionalArmor.GetValue(); // additional armor gives +1 armor
        float totalArmor = baseArmor + bonusArmor;

        float reductionMultipler = Mathf.Clamp01(1 - armorReduction);
        float effectiveArmor = totalArmor * reductionMultipler;

        float mitigation = effectiveArmor / (effectiveArmor + 100);
        float mitigationCap = .85f; // Max matigation is 85%
        float finalMitigation = Mathf.Clamp(mitigation, 0, mitigationCap);

        return finalMitigation;
    }

    public float GetMaxHealth()
    {
        float baseMaxHealth = resources.maxHealth.GetValue();
        float bonusMaxHealth = major.vitality.GetValue() * 25;

        float finalMaxHealth = baseMaxHealth + bonusMaxHealth;
        return finalMaxHealth;
    }

    public float GetArmorReduction()
    {
        float baseArmorReduction = offense.armorReduction.GetValue(); // armor reduction is a multiplier eg (30 / 100 = 0.3f)
        float bonusArmorReduction = bonus.additionalArmorReduction.GetValue();
        float finalArmorReduction = (baseArmorReduction + bonusArmorReduction) / 100;

        return finalArmorReduction;
    }


    public Stat GetStatByType(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHealth: return resources.maxHealth;
            case StatType.HealthRegen: return resources.healthRegen;

            case StatType.BonusHealthRegen: return regen.bonusHealthRegen;
            case StatType.RegenCooldownReduction: return regen.regenCooldownReduction;

            case StatType.Damage: return offense.damage;
            case StatType.CritPower: return offense.critPower;
            case StatType.CritChance: return offense.critChance;
            case StatType.AttackSpeed: return offense.attackSpeed;
            case StatType.AttackCooldown: return offense.attackCooldown;
            case StatType.ArmorReduction: return offense.armorReduction;

            case StatType.Strength: return major.strength;
            case StatType.Vitality: return major.vitality;

            case StatType.AdditionalCritPower: return bonus.additionalCritPower;
            case StatType.AdditionalArmor: return bonus.additionalArmor;
            case StatType.AdditionalArmorReduction: return bonus.additionalArmorReduction;
            case StatType.AdditionalCritChance: return bonus.additionalCritChance;

            default:
                Debug.LogWarning($"StatType {type} not implemented yet.");
                return null;
        }
    }

    [ContextMenu("Update Default Stat Setup")]
    public void ApplyDefaultStatSetup()
    {
        if(defaultStatSetup == null)
        {
            Debug.Log("No default stat setup assigned");
            return;
        }

        resources.maxHealth.SetBaseValue(defaultStatSetup.maxHealth);
        resources.healthRegen.SetBaseValue(defaultStatSetup.healthRegen);

        //Add more here
    }
}

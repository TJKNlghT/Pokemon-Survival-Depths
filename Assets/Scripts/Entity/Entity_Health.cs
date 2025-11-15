using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class Entity_Health : MonoBehaviour, IDamagable
{
    private Entity_VFX entityVfx;
    private Entity entity;
    private Entity_Stats entityStats;

    [Header("UI")]
    [SerializeField] private Slider healthBar;

    [SerializeField] protected float currentHealth;
    [SerializeField] protected bool isDead;

    [Header("Health Regen")]
    [SerializeField] private bool canRegenerateHealth = true;
    [SerializeField] private float regenTickInterval = 1f;   // how often a tick happens
    [SerializeField] private float baseRegenCooldown = 5f;   // delay after taking dmg before regen can start

    private Coroutine regenCo;
    private Coroutine regenCooldownCo;
    private bool regenOnCooldown;


    [Header("On Damage Knockback")]
    [SerializeField] private Vector2 knockbackPower = new Vector2(4f, 4f);
    [SerializeField] private Vector2 heavyKnockbackPower = new Vector2(7, 7);
    [SerializeField] private float knockbackDuration = .2f;
    [SerializeField] private float heavyKnockbackDuration = .5f;

    [Header("On Heavy Damage")]
    [SerializeField] private float heavyDamageThreshold = .3f; // Percentage of health you lose to be considered heavy damage

    protected virtual void Awake()
    {
        entityVfx = GetComponent<Entity_VFX>();
        entity = GetComponent<Entity>();
        entityStats = GetComponent<Entity_Stats>();

        if (!healthBar)
            healthBar = GetComponentInChildren<Slider>();

        currentHealth = entityStats.GetMaxHealth();
        UpdateHealthBar();
    }

    public void BindHealthBar(Slider slider)
    {
        healthBar = slider;
        UpdateHealthBar();
    }

    public bool IsAtFullHealth()
    {
        float maxHealth = entityStats.GetMaxHealth();
        return currentHealth >= maxHealth;
    }

    public virtual void TakeDamage(float damage, Transform damageDealer)
    {
        if (isDead)
            return;

        // stop regen & start cooldown
        OnTookDamage();

        Entity_Stats attackerStats = damageDealer.GetComponent<Entity_Stats>();
        float armorReduction = attackerStats != null ? attackerStats.GetArmorReduction() : 0;

        float mitigation = entityStats.GetArmorMitigation(armorReduction);
        float finalDamage = damage * (1 - mitigation); // example: 150 damage & 60% mitigation will reduce it to 60 dmg

        TakeKnockback(damageDealer, finalDamage);
        ReduceHealth(finalDamage);
    }

    private void OnTookDamage()
    {
        // cancel active regen
        if (regenCo != null)
        {
            StopCoroutine(regenCo);
            regenCo = null;
        }

        // restart cooldown
        if (regenCooldownCo != null)
            StopCoroutine(regenCooldownCo);

        regenOnCooldown = true;
        regenCooldownCo = StartCoroutine(RegenCooldownCo());
    }

    private IEnumerator RegenCooldownCo()
    {
        float cd = GetRegenCooldown();
        yield return new WaitForSeconds(cd);
        regenOnCooldown = false;

        TryStartRegen();
    }

    private float GetRegenCooldown()
    {
        float cd = baseRegenCooldown;

        if (entityStats != null)
        {
            float pct = entityStats.regen.regenCooldownReduction.GetValue(); // 0 - 1
            cd *= Mathf.Max(0.1f, 1f - pct / 100f);
        }

        return cd;
    }

    private void TryStartRegen()
    {
        if (!canRegenerateHealth || isDead)
            return;

        if (IsAtFullHealth())
            return;

        if (regenOnCooldown)
            return;

        if (regenCo == null)
            regenCo = StartCoroutine(RegenCo());
    }

    private IEnumerator RegenCo()
    {
        while (!isDead && canRegenerateHealth)
        {
            if (IsAtFullHealth())
                break;

            float regenAmount = GetHealthRegenPerTick();
            IncreaseHealth(regenAmount);

            yield return new WaitForSeconds(regenTickInterval);
        }

        regenCo = null;
    }

    private float GetHealthRegenPerTick()
    {
        // base regen
        float baseRegen = entityStats.resources.healthRegen.GetValue();

        // bonus regen
        if (entityStats != null)
            baseRegen += entityStats.regen.bonusHealthRegen.GetValue();

        return baseRegen;
    }

    public void IncreaseHealth(float healAmount)
    {
        if (isDead)
            return;

        float newHealth = currentHealth + healAmount;
        float maxHealth = entityStats.GetMaxHealth();

        currentHealth = Mathf.Min(newHealth, maxHealth);
        UpdateHealthBar();
    }

    public void ReduceHealth(float damage)
    {
        entityVfx?.PlayOnDamageVfx();
        currentHealth -= damage;
        UpdateHealthBar();

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;        //additional safety
        isDead = true;

        // stop regen completely on death
        if (regenCo != null)
            StopCoroutine(regenCo);
        if (regenCooldownCo != null)
            StopCoroutine(regenCooldownCo);

        entity.EntityFaint();
    }

    public void ClampToMax()
    {
        float maxHealth = entityStats.GetMaxHealth();
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (!healthBar || entityStats == null)
            return;

        healthBar.value = currentHealth / entityStats.GetMaxHealth();
    }

    public void ResetFullHealth()
    {
        isDead = false;
        currentHealth = entityStats.GetMaxHealth();
        UpdateHealthBar();
    }

    private void TakeKnockback(Transform damageDealer, float finalDamage)
    {
        Vector2 knockback = CalculateKnockback(finalDamage, damageDealer);
        float duration = CalculateDuration(finalDamage);

        entity?.ReceiveKnockback(knockback, duration);
    }


    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {
        if (damageDealer == null || entity == null || entity.rb == null)
            return Vector2.zero;

        // direction of incoming force
        Vector2 dir = entity.rb.position - (Vector2)damageDealer.position;

        if (dir.sqrMagnitude < 0.0001f)
        {
            // fallback if positions are almost identical
            dir = entity.lastDir.sqrMagnitude > 0.01f ? entity.lastDir : Vector2.up;
        }

        dir.Normalize();

        // scale per axis by knockbackPower
        return new Vector2(
            dir.x * (IsHeavyDamage(damage) ? heavyKnockbackPower.x : knockbackPower.x),
            dir.y * (IsHeavyDamage(damage) ? heavyKnockbackPower.y : knockbackPower.y)
        );
    }

    private float CalculateDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;

    private bool IsHeavyDamage(float damage) => damage / entityStats.GetMaxHealth() > heavyDamageThreshold;
}

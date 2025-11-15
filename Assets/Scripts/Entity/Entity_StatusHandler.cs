using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class Entity_StatusHandler : MonoBehaviour
{
    private Entity entity;
    private Entity_VFX entityVfx;
    private Entity_Health entityHealth;
    private ElementType currentStatus = ElementType.Normal;

    [Header("Shock Effect Details")]
    [SerializeField] private GameObject lightningStrikeVfx;
    [SerializeField] private float currentCharge;
    [SerializeField] private float maximumCharge = 3;

    private Coroutine shockCo;

    [Header("Status Immunity")]
    [SerializeField] private float statusImmunityDuration = 2f; // time you stay immune after status ends

    private bool isImmune;
    private Coroutine statusCo;
    private Coroutine immunityCo;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
        entityHealth = GetComponent<Entity_Health>();
    }

    public void ApplyStatusEffect(ElementType element, ElementalEffectData effectData)
    {
        if(element == ElementType.Water && CanBeApplied(ElementType.Water))
        {
            ApplySlowEffect(effectData.slowDuration, effectData.slowMultiplier);
        }

        if (element == ElementType.Fire && CanBeApplied(ElementType.Fire))
        {
            ApplyBurnEffect(effectData.burnDuration, effectData.burnDamage);
        }

        if (element == ElementType.Electric && CanBeApplied(ElementType.Electric))
        {
            ApplyShockEffect(effectData.shockDuration, effectData.shockDamage, effectData.shockCharge);
        }
    }

    public void ApplyShockEffect(float duration, float damage, float charge)
    {
        if (!CanBeApplied(ElementType.Electric))
            return;

        currentCharge += charge;

        if (currentCharge >= maximumCharge)
        {
            // full charge: strike, clear status, start immunity
            DoLightningStrike(damage);
            StopShockEffect(startImmunity: true);
            return;
        }

        if (shockCo != null)
            StopCoroutine(shockCo);

        shockCo = StartCoroutine(ShockEffectCo(duration));
    }

    private IEnumerator ShockEffectCo(float duration)
    {
        currentStatus = ElementType.Electric;

        entityVfx.PlayOnStatusVfx(duration, ElementType.Electric);

        yield return new WaitForSeconds(duration);

        // electric status ends without full strike
        StopShockEffect(startImmunity: true);
    }

    private void StopShockEffect(bool startImmunity)
    {
        if (shockCo != null)
        {
            StopCoroutine(shockCo);
            shockCo = null;
        }

        currentStatus = ElementType.Normal;
        currentCharge = 0f;

        entityVfx.StopAllVfx();

        if (startImmunity)
            StartImmunity();
    }

    private void DoLightningStrike(float damage)
    {
        if (lightningStrikeVfx != null)
            Instantiate(lightningStrikeVfx, transform.position, Quaternion.identity);

        entityHealth.ReduceHealth(damage);
    }

    public void ApplyBurnEffect(float duration, float totalDamage)
    {
        if (!CanBeApplied(ElementType.Fire))
            return;

        if (statusCo != null)
            StopCoroutine(statusCo);

        statusCo = StartCoroutine(BurnEffectCo(duration, totalDamage));
    }

    private IEnumerator BurnEffectCo(float duration, float totalDamage)
    {
        currentStatus = ElementType.Fire;
        entityVfx.PlayOnStatusVfx(duration, ElementType.Fire);

        int ticksPerSecond = 2;
        int tickCount = Mathf.RoundToInt(ticksPerSecond * duration);

        float damagePerTick = totalDamage / tickCount;
        float tickInterval = 1f / ticksPerSecond;

        for(int i = 0; i < tickCount; i++)
        {
            entityHealth.ReduceHealth(damagePerTick);

            yield return new WaitForSeconds(tickInterval);
        }

        currentStatus = ElementType.Normal;

        StartImmunity();
    }



    public void ApplySlowEffect(float duration, float slowMultiplier)
    {
        if (!CanBeApplied(ElementType.Water))
            return;

        if (statusCo != null)
            StopCoroutine(statusCo);

        statusCo = StartCoroutine(SlowEffectCo(duration, slowMultiplier));
    }

    private IEnumerator SlowEffectCo(float duration, float slowMultiplier)
    {
        currentStatus = ElementType.Water;

        //slow the entity
        entity.SlowDownEntityBy(duration, slowMultiplier);

        // play some status VFX
        entityVfx?.PlayOnStatusVfx(duration, ElementType.Water);

        yield return new WaitForSeconds(duration);

        // status ends
        currentStatus = ElementType.Normal;

        // start immunity window after the status is gone
        StartImmunity();
    }

    private void StartImmunity()
    {
        if (immunityCo != null)
            StopCoroutine(immunityCo);
        immunityCo = StartCoroutine(StatusImmunityCo());
    }

    private IEnumerator StatusImmunityCo()
    {
        isImmune = true;
        yield return new WaitForSeconds(statusImmunityDuration);
        isImmune = false;
    }

    public bool CanBeApplied(ElementType element)
    {
        if (element == ElementType.Electric)
        {
            return !isImmune;
        }

        return currentStatus == ElementType.Normal && !isImmune;
    }
}

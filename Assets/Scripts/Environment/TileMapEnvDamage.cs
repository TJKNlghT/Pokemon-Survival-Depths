using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TileMapEnvDamage : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float damagePerTick = 5f;
    [SerializeField] private float tickInterval = 1f;

    [Header("Filter")]
    [SerializeField] private bool onlyAffectPlayer = true;
    [SerializeField] private LayerMask whatIsVictim;

    private readonly Dictionary<IDamagable, float> _nextDamageTime = new();

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = false; // solid cactus wall
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var other = collision.collider;

        if (onlyAffectPlayer && !other.CompareTag("Player"))
            return;

        if (whatIsVictim.value != 0 &&
            ((1 << other.gameObject.layer) & whatIsVictim.value) == 0)
            return;

        var damagable = other.GetComponent<IDamagable>();
        if (damagable == null)
            return;

        float now = Time.time;
        if (_nextDamageTime.TryGetValue(damagable, out float nextTime) && now < nextTime)
            return;

        damagable.TakeDamage(damagePerTick, transform);
        _nextDamageTime[damagable] = now + tickInterval;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        var damagable = collision.collider.GetComponent<IDamagable>();
        if (damagable != null)
            _nextDamageTime.Remove(damagable);
    }
}
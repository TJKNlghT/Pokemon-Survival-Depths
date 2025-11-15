using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnvDamageOnce : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float damage = 10f;

    [Header("Filter")]
    [SerializeField] private bool onlyAffectPlayer = true;
    [SerializeField] private LayerMask whatIsVictim;  // optional extra filter

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true; // one-shot hazard uses trigger
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1) Only player?
        if (onlyAffectPlayer && !other.CompareTag("Player"))
            return;

        // 2) Optional layer filter
        if (whatIsVictim.value != 0 &&
            ((1 << other.gameObject.layer) & whatIsVictim.value) == 0)
            return;

        // 3) Something that can take damage?
        var damagable = other.GetComponent<IDamagable>();
        if (damagable == null)
            return;

        // 4) Apply damage once
        damagable.TakeDamage(damage, transform);

        // 5) Optional: destroy hazard after it fires once
        // Destroy(gameObject);
    }
}
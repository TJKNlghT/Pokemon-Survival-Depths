using System.Collections;
using UnityEngine;

public abstract class TimedPickUps : PickUps
{
    [Header("Buff Details")]
    [SerializeField] protected float buffDuration = 5f;

    protected override void Awake()
    {
        base.Awake();
        // buff object destroys itself at the end of the buff
        destroyOnPickup = false;
    }

    protected override bool TryApplyPickup(Player player)
    {

        if (!CanApplyTo(player))
            return false;

        canBePicked = false;
        if (col) col.enabled = false;
        if (sr) sr.enabled = false;

        StartCoroutine(BuffCo(player));
        return true; // pickup was "consumed" logically, even though we destroy later
    }

    protected virtual bool CanApplyTo(Player player) => true; // future use (if theres buff and we dont want the same buff applied

    private IEnumerator BuffCo(Player player)
    {
        OnBuffStart(player);

        yield return new WaitForSeconds(buffDuration);

        OnBuffEnd(player);

        Destroy(gameObject);
    }

    protected abstract void OnBuffStart(Player player);
    protected abstract void OnBuffEnd(Player player);
}
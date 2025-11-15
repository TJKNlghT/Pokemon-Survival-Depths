using UnityEngine;

public class PickUps_Potion : PickUps
{
    [SerializeField] private float healAmount = 25f;

    protected override bool TryApplyPickup(Player player)
    {
        var health = player.GetComponent<Entity_Health>();
        if (health == null)
            return false;

        // If already full, will not pickup
        if (health.IsAtFullHealth())
            return false;

        health.IncreaseHealth(healAmount);
        return true; // consumed
    }
}

using UnityEngine;

public class Enemy_Health : Entity_Health
{
    public override void TakeDamage(float damage, Transform damageDealer)
    {
        base.TakeDamage(damage, damageDealer);

        //Any additional stuff
    }
}

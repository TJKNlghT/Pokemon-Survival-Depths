using UnityEngine;

public class Enemy_Combat : Entity_Combat
{
    private Enemy_VFX enemyVfx;

    protected override void Awake()
    {
        base.Awake();
        enemyVfx = GetComponent<Enemy_VFX>();
    }

    protected override void HandlePostHitVfx(Collider2D target)
    {
        if (enemyVfx != null)
        {
            enemyVfx.UpdateOnHitColor();
            enemyVfx.CreateOnHitVFX(target);
        }
        else
        { 
            base.HandlePostHitVfx(target);
        }
    }
}
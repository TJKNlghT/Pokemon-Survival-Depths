using UnityEngine;

public class Entity_AnimationTriggers : MonoBehaviour
{
    private Entity entity;
    private Entity_Combat entityCombat;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entityCombat = GetComponentInParent<Entity_Combat>();
    }
    private void CurrentStateTrigger()
    {
        // get access to entity and let current entity's state know we want to exit state
        entity.CurrentStateAnimationTrigger();
    }

    private void AttackTrigger()
    {
        entityCombat.PerformAttack();
    }

    private void ProjectileAttackTrigger() => entityCombat.ShootProjectile();
}

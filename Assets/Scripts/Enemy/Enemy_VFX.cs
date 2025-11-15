using UnityEngine;

public class Enemy_VFX : Entity_VFX
{

    private Color originalHitVfxColor;

    protected override void Awake()
    {
        base.Awake();
        originalHitVfxColor = hitVfxColor;
    }

    public void UpdateOnHitColor()
    {
        ElementType element = stats != null ? stats.Element : ElementType.Normal;

        switch (element)
        {
            case ElementType.Fire:
                hitVfxColor = fireVfx; break;
            case ElementType.Water:
                hitVfxColor = waterVfx; break;
            case ElementType.Electric:
                hitVfxColor = electricVfx; break;
            default:
                hitVfxColor = normalVfx; break;
        }
    }
}
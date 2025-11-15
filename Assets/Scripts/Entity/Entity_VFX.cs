using System.Buffers.Text;
using System.Collections;
using TMPro;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    private SpriteRenderer sr;
    private Animator anim;
    private int animPauseCount = 0;
    protected Entity_Stats stats;

    [Header("On Damage VFX")]
    [SerializeField] private Material onDamageMaterial;
    [SerializeField] private float onDamageVfxDuration = .1f;

    [Header("On Faint VFX")]
    [SerializeField] private float faintFlickerDuration = 1.5f;
    [SerializeField] private float faintFlickerInterval = 0.15f;

    [Header("On Knockback VFX")]
    [SerializeField] private Sprite[] hurtSprites = new Sprite[8];

    private Material originalMaterial;
    private Sprite originalSprite;
    private Coroutine onDamageVfxCoroutine;
    private Coroutine faintVfxCoroutine;
    private Coroutine swapSpriteCoroutine;
    private Coroutine statusVfxCoroutine;

    [Header("On Doing Damage VFX")]
    [SerializeField] private GameObject hitVfx;
    [SerializeField] protected Color hitVfxColor = Color.white;

    [Header("Element colors")]
    [SerializeField] protected Color normalVfx = Color.white;
    [SerializeField] protected Color fireVfx = Color.orangeRed;
    [SerializeField] protected Color waterVfx = Color.lightCyan;
    [SerializeField] protected Color electricVfx = Color.lightYellow;

    protected virtual void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        stats = GetComponent<Entity_Stats>();
        originalMaterial = sr.material;
        originalSprite = sr.sprite;
    }

    public void PlayOnStatusVfx(float duration, ElementType element)
    {
        Color color;

        if (element == ElementType.Water) color = waterVfx;
        else if (element == ElementType.Fire) color = fireVfx;
        else if (element == ElementType.Electric) color = electricVfx;
        else return;

        if (statusVfxCoroutine != null)
            StopCoroutine(statusVfxCoroutine);

        statusVfxCoroutine = StartCoroutine(PlayStatusVfxCo(duration, color));
    }

    private IEnumerator PlayStatusVfxCo(float duration, Color effectColor)
    {
        float tickInterval = .25f;
        float timeHasPassed = 0;

        Color lightColor = effectColor * 1.2f;
        Color darkColor = effectColor * .8f;

        bool toggle = false;

        while (timeHasPassed < duration)
        {
            sr.color = toggle ? lightColor : darkColor;
            toggle = !toggle;

            yield return new WaitForSeconds(tickInterval);
            timeHasPassed += tickInterval;
        }

        sr.color = Color.white;
        statusVfxCoroutine = null;
    }

    public void StopAllVfx()
    {
        // stop known VFX coroutines individually
        if (onDamageVfxCoroutine != null)
        {
            StopCoroutine(onDamageVfxCoroutine);
            onDamageVfxCoroutine = null;
        }

        if (faintVfxCoroutine != null)
        {
            StopCoroutine(faintVfxCoroutine);
            faintVfxCoroutine = null;
        }

        if (statusVfxCoroutine != null)
        {
            StopCoroutine(statusVfxCoroutine);
            statusVfxCoroutine = null;
        }

        // if we interrupt hurt-sprite, manually restore it and animator
        if (swapSpriteCoroutine != null)
        {
            StopCoroutine(swapSpriteCoroutine);
            swapSpriteCoroutine = null;

            sr.sprite = originalSprite;
            animPauseCount = 0;
            if (anim) anim.enabled = true;
        }

        sr.color = Color.white;
        sr.material = originalMaterial;
    }

public void CreateOnHitVFX(Collider2D target)
    {
        if (hitVfx == null || target == null) return;

        Vector3 pos = target.bounds.center;   // world-space center of the collider
        pos.z = 0f;                              // make sure it’s on your 2D plane

        GameObject vfx = Instantiate(hitVfx, pos, Quaternion.identity);
        vfx.GetComponentInChildren<SpriteRenderer>().color = hitVfxColor;
    }

    public void PlayOnDamageVfx()
    {
        if(onDamageVfxCoroutine != null)
        {
            StopCoroutine(onDamageVfxCoroutine);
        }

        onDamageVfxCoroutine = StartCoroutine(OnDamageVfxCo());
    }
    private IEnumerator OnDamageVfxCo()
    {
        sr.material = onDamageMaterial;

        yield return new WaitForSeconds(onDamageVfxDuration);

        sr.material = originalMaterial;
    }

    public void PlayFaintVfxAndDisappear()
    {
        if (faintVfxCoroutine != null)
            StopCoroutine(faintVfxCoroutine);

        faintVfxCoroutine = StartCoroutine(FaintVfxCo());
    }

    private IEnumerator FaintVfxCo()
    {
        var dropper = GetComponent<Enemy_Drop>();
        float elapsed = 0f;
        bool visible = true;

        // stop any damage flash
        if (onDamageVfxCoroutine != null)
        {
            StopCoroutine(onDamageVfxCoroutine);
            sr.material = originalMaterial;
        }

        while (elapsed < faintFlickerDuration)
        {
            visible = !visible;
            sr.enabled = visible;

            yield return new WaitForSeconds(faintFlickerInterval);
            elapsed += faintFlickerInterval;
        }

        sr.enabled = false;
        dropper?.TryDrop();
        Destroy(transform.gameObject);
    }

    public void ShowHurtSprite(float seconds, Vector2 dir)
    {
        if (swapSpriteCoroutine != null)
        {
            StopCoroutine(swapSpriteCoroutine);
            ResumeAnimator(); // re-enable after killing the old one
        }
        swapSpriteCoroutine = StartCoroutine(SwapSpriteCo(seconds, dir));
    }

    private IEnumerator SwapSpriteCo(float seconds, Vector2 dir)
    {
        if (sr == null) yield break;

        PauseAnimator();

        var prev = sr.sprite;
        sr.sprite = GetHurtSpriteFor(dir) ?? prev;

        try
        {
            yield return new WaitForSeconds(seconds);
        }
        finally
        {
            sr.sprite = prev;
            ResumeAnimator(); // if coroutine is stopped
        }
    }

    private Sprite GetHurtSpriteFor(Vector2 dir)
    {
        if (hurtSprites == null || hurtSprites.Length < 8) return null;

        if (dir.sqrMagnitude < 1e-6f) dir = Vector2.down;
        dir.Normalize();

        // 8 canonical unit directions, in the SAME order as your inspector array:
        // N, NE, E, SE, S, SW, W, NW
        Vector2[] canon =
        {
        new Vector2( 0, 1), // N
        new Vector2( 1, 1).normalized, // NE
        new Vector2( 1, 0), // E
        new Vector2( 1,-1).normalized, // SE
        new Vector2( 0,-1), // S
        new Vector2(-1,-1).normalized, // SW
        new Vector2(-1, 0), // W
        new Vector2(-1, 1).normalized  // NW
    };

        int best = 0;
        float bestDot = -1f;
        for (int i = 0; i < 8; i++)
        {
            float d = Vector2.Dot(dir, canon[i]);
            if (d > bestDot) { bestDot = d; best = i; }
        }

        return hurtSprites[best];
    }

    void PauseAnimator()
    {
        if (!anim) return;
        animPauseCount++;
        anim.enabled = false;
    }

    void ResumeAnimator()
    {
        if (!anim) return;
        animPauseCount = Mathf.Max(0, animPauseCount - 1);
        if (animPauseCount == 0) anim.enabled = true;
    }

    void OnDisable()
    {
        // safety: never leave it paused
        if (anim) anim.enabled = true;
        animPauseCount = 0;
    }
}

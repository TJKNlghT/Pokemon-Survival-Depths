using NUnit.Framework.Constraints;
using System;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private const int DEFAULT_GRAVITY = 0;
    private const float DEFAULT_COLLISION_RADIUS = 0.4f;
    private const float DEFAULT_DETECTION_DISTANCE = 0.8f;

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Entity_Stats stats { get; private set; }

    protected StateMachine stateMachine;

    public Vector2 lastDir { get; set; } = Vector2.down; //Default facing is down
    public CircleCollider2D bodyCircle { get; private set; }

    [Header("Collision Detection")]
    [SerializeField] private float detectionRadius = DEFAULT_COLLISION_RADIUS;
    [SerializeField] private float detectionDistance = DEFAULT_DETECTION_DISTANCE;
    [SerializeField] public LayerMask whatIsObstacle {  get; protected set; }


    [Header("Behaviour")]
    [SerializeField] private bool isProjectileAttacker = false;
    public bool IsProjectileAttacker => isProjectileAttacker;

    //Status Effects
    private bool isKnocked;
    private Coroutine knockbackCo;
    private Coroutine slowDownCo;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        bodyCircle = GetComponent<CircleCollider2D>();
        stats = GetComponent<Entity_Stats>();

        rb.gravityScale = DEFAULT_GRAVITY;

        whatIsObstacle = LayerMask.GetMask("Obstacle");

        stateMachine = new StateMachine();
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        stateMachine.UpdateActiveState();
    }

    protected virtual void FixedUpdate()
    {
        stateMachine.PhysicsUpdateActiveState();
    }

    public void CurrentStateAnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();
    }

    public void ReceiveKnockback(Vector2 knockback, float duration)
    {
        if (knockbackCo != null)
        {
            StopCoroutine(knockbackCo);
        }

        knockbackCo = StartCoroutine(KnockbackCo(knockback, duration));
    }

    private IEnumerator KnockbackCo(Vector2 knockback, float duration)
    {
        isKnocked = true;
        rb.linearVelocity = knockback;


        var vfx = GetComponent<Entity_VFX>();
        vfx?.ShowHurtSprite(duration, lastDir);

        yield return new WaitForSeconds(duration);

        rb.linearVelocity = Vector2.zero;

        isKnocked = false;
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnocked)
            return;

        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
    }

    public virtual bool HandleCollisionDetection(Vector2 dir, out Collider2D hit)
    {
        hit = null;

        if (dir.sqrMagnitude < 0.01f)
            return false;

        dir = dir.normalized;

        Vector2 currentPos = rb.position;
        Vector2 checkPos = currentPos + dir * detectionDistance;

        // only obstacles block movement
        hit = Physics2D.OverlapCircle(checkPos, detectionRadius, whatIsObstacle);

        if (hit == null)
            return false;

        return true;
    }

    public virtual void EntityFaint()
    {
    }

    public virtual void SlowDownEntityBy(float duration, float slowMultiplier)
    {
        if (slowDownCo != null)
            StopCoroutine(slowDownCo);

        slowDownCo = StartCoroutine(SlowDownEntityCo(duration, slowMultiplier));
    }

    protected virtual IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        yield return null;
    }

    private void OnDrawGizmos()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        Vector2 basePos = rb != null ? rb.position : (Vector2)transform.position;

        Vector2 dir = lastDir.sqrMagnitude > 0.01f ? lastDir.normalized : Vector2.down;
        Vector2 checkPos = basePos + dir * detectionDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(checkPos, detectionRadius);

        if (bodyCircle == null)
            bodyCircle = GetComponent<CircleCollider2D>();

        if (bodyCircle != null)
        {
            Vector2 center = (Vector2)transform.TransformPoint(bodyCircle.offset);
            float radius = bodyCircle.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, radius);
        }
    }
}

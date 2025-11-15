using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class PickUps : MonoBehaviour
{
    protected SpriteRenderer sr;
    protected Collider2D col;

    [Header("Float FX")]
    [SerializeField] private float floatSpeed = 2.2f;
    [SerializeField] private float floatRange = .1f;

    [Header("Pickup Settings")]
    [SerializeField] protected bool canBePicked = true;
    [SerializeField] protected bool destroyOnPickup = true;

    private Vector3 startPosition;

    protected virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        startPosition = transform.position;
    }

    protected virtual void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
        transform.position = startPosition + new Vector3(0, yOffset);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBePicked)
            return;

        if (!collision.CompareTag("Player"))
            return;

        Player player = collision.GetComponent<Player>();
        if (player == null)
            return;

        // let subclass decide if pickup was actually consumed
        bool consumed = TryApplyPickup(player);

        if (consumed && destroyOnPickup)
            Destroy(gameObject);
    }
    protected abstract bool TryApplyPickup(Player player);

}

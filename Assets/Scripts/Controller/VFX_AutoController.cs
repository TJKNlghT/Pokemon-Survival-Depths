using UnityEngine;

public class VFX_AutoController : MonoBehaviour
{
    [SerializeField] private bool autoDestroy = true;
    [SerializeField] private float destroyDelay = 0.5f;

    private void Start()
    {
        if (autoDestroy)
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}

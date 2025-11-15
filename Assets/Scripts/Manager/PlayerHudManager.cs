using UnityEngine;
using UnityEngine.UI;

public class PlayerHudManager : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider xpSlider;

    private void Awake()
    {
        // Auto-find by name if not assigned in inspector
        if (!hpSlider || !xpSlider)
        {
            var sliders = GetComponentsInChildren<Slider>(true);
            foreach (var s in sliders)
            {
                var n = s.gameObject.name.ToLower();
                if (!hpSlider && n.Contains("hp"))
                    hpSlider = s;
                else if (!xpSlider && n.Contains("xp"))
                    xpSlider = s;
            }
        }
    }

    private void Start()
    {
        var player = FindFirstObjectByType<Player>();
        if (!player)
        {
            Debug.LogWarning("[PlayerHUD_Binder] No Player found in scene.");
            return;
        }

        var health = player.GetComponent<Entity_Health>();
        var xp = player.GetComponent<Player_XP>();

        if (health && hpSlider)
            health.BindHealthBar(hpSlider);

        if (xp && xpSlider)
            xp.BindXPBar(xpSlider);
    }
}
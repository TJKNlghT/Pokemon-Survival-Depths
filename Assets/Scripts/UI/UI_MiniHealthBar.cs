using UnityEngine;

public class UI_MiniHealthBar : MonoBehaviour
{
    //For managing many healthbar ui mini at the same time (using event)
    private Entity entity => GetComponentInParent<Entity>();

    //Subscriber
    private void OnEnable()
    {
        
    }

    //Unsubscriber
    private void OnDisable()
    {
        
    }
}

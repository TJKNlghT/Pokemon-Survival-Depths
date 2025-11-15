using UnityEngine;

public class PickUps_XP : PickUps
{
    [SerializeField] private int xpAmount = 10;

    protected override bool TryApplyPickup(Player player)
    {
        var xp = player.GetComponent<Player_XP>();
        if (xp == null)
        {
            Debug.LogWarning("PickUps_XP: Player_XP component not found on Player.");
            return false; // don't consume pickup if we can't apply XP
        }

        xp.AddXP(xpAmount);
        return true; // pickup consumed
    }
}
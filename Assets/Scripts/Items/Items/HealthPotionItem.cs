using FloatingText;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotionItem : Item
{
    ///The amount the potion heals
    [Range(0,100)]public int healthPercent = 30;
    ///Pop-up text when used
    public FloatingTextValues healthFloatingText;

    ///Heal the Player
    public override bool Use()
    {
        Character player = GameManager.instance.player;
        if (!player.IsMaxHealth())
        {
            player.HealPercent(healthPercent);
            FloatingTextManager.instance.SetStationaryFloatingText(healthFloatingText, player.transform.position);
            return true;
        }
        return false;
    }
}

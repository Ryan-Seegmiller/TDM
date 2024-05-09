using FloatingText;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldItem : Item
{
    /// Gold amount to add
    public int goldAmount = 1;
    /// Pop-up text
    public FloatingTextValues goldText;

    ///Add to inventory and show floating text
    public override void Pickup()
    {
        FloatingTextManager.instance.SetStaticMovementFloatingText(GetGoldValues(goldAmount), HUDManager.instance.progressionValues.goldText.rectTransform, GameManager.instance.mainCamera.WorldToScreenPoint(transform.position),() => { GameManager.instance.AddGold(goldAmount); });
        base.Pickup();
    }

    /// <summary> Assign the right values to floating text </summary>
    /// <param name="gold"></param>
    /// <returns></returns>
    private FloatingTextValues GetGoldValues(int gold)
    {
        string goldText = $" {gold} {this.goldText.text}";
        FloatingTextValues goldTextValues = new FloatingTextValues();
        goldTextValues.text = goldText;
        goldTextValues.textColor = this.goldText.textColor;
        goldTextValues.duration = this.goldText.duration;
        goldTextValues.speed = this.goldText.speed;
        goldTextValues.offset = this.goldText.offset;
        goldTextValues.direction = this.goldText.direction;
        goldTextValues.hasStaticMovement = this.goldText.hasStaticMovement;
        goldTextValues.size = this.goldText.size;
        goldTextValues.momentaryHoverFloat = this.goldText.momentaryHoverFloat;
        return goldTextValues;
    }
}

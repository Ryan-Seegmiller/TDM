using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Archer-specific ammo bar </summary>
public class QuiverManager : Bar
{
    [SerializeField] Image targetImage;
    ///ordered sprites
    [SerializeField] Sprite[] sprites = new Sprite[11];

    private void Start()
    {
        if(sprites != null && sprites.Length > 0) { targetImage.sprite = sprites[0]; }
    }

    /// <summary> The archer cycles sprites instead of changing a slider </summary>
    /// <param name="currentValue"></param>
    /// <param name="maxValue"></param>
    /// <param name="flash"></param>
    public override void ChangeValue(int currentValue, int maxValue, bool flash = true)
    {
        //percentage 0 to 1
        float nextValue = (float)currentValue / maxValue;
        //set index 0 to 10
        if (sprites != null && sprites.Length > 0) 
        { targetImage.sprite = sprites[(int)(nextValue * 10)]; }
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
        if (valueText != null) { valueText.text = currentValue.ToString(); }
    }
}

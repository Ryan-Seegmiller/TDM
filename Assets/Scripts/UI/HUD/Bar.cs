using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary> A slider that adjusts to percent values and flashes when changed </summary>
public class Bar : MonoBehaviour
{
    ///Visual fill amount
    public Slider Stat;

    ///Numerical stat
    public TextMeshProUGUI valueText;
    ///Visual flash
    public Image FlashingBox;

    ///Color to flash when decreased
    public Color negativeColor;
    ///Color to flash when increased
    public Color positveColor;

    ///Time spent per flash in seconds
    public float flashTime = 0.5f;
    ///Cached for stopping early
    Coroutine flashing = null;

    /// <summary> Adjusts visual slider and flashes colors when value changes </summary>
    /// <param name="currentValue"></param>
    /// <param name="maxValue"></param>
    /// <param name="flash"></param>
    public virtual void ChangeValue(int currentValue, int maxValue, bool flash = true)
    {
        float nextValue = (float)currentValue / maxValue;
        if (flash)
        {
            if (nextValue < Stat.value)
            {
                //trigger negative anim
                if (flashing != null)
                {
                    StopFlashing();
                    flashing = StartCoroutine(Flash(negativeColor));
                }
                else
                {
                    flashing = StartCoroutine(Flash(negativeColor));
                }
            }
            else
            {
                //trigger positive anim
                if (flashing != null)
                {
                    StopFlashing();
                    flashing = StartCoroutine(Flash(positveColor));
                }
                else
                {
                    flashing = StartCoroutine(Flash(positveColor));
                }
            }
        }
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
        if (Stat != null) { Stat.value = nextValue; }
        if (valueText != null) { valueText.text = currentValue.ToString(); }
    }

    /// <summary> Flashes the bar a specified color </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    IEnumerator Flash(Color color)
    {
        Color originalColor = FlashingBox.color;
        FlashingBox.color = color;
        yield return new WaitForSeconds(flashTime);
        FlashingBox.color = originalColor;
        flashing = null;
    }
    ///Ends current flash
    private void StopFlashing()
    {
        StopCoroutine(flashing);
        FlashingBox.color = Color.clear;
    }
}

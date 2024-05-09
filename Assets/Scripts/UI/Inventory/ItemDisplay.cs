using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Item display
/// </summary>
public class ItemDisplay : MonoBehaviour
{
    #region Declerations
    /// <summary>Stores the item data asociated</summary>
    public ItemData data;
    /// <summary>Img for the item sprite</summary>
    Image img;
    /// <summary>COunt for the display amount of items</summary>
    public TextMeshProUGUI count;
    /// <summary>the amount of items</summary>
    int amount = 0;
    /// <summary>
    /// Returns the amount amount of items in the slot
    /// </summary>
    public int amt
    {
        get { return amount; }
        set
        {
            amount = value;
            if(count != null) count.text = "x" + amount.ToString();
        }
    }
    #endregion

    #region MonoBehaviours
    void Awake()
    {
        img = GetComponent<Image>();
        Refresh(null, 0);
    }
    #endregion

    #region Refresh
    /// <summary>
    /// Refreshes the diaplay of the item
    /// </summary>
    /// <param name="iData"></param>
    /// <param name="amount"></param>
    public void Refresh(ItemData iData, int amount)
    {
        if(iData != null) { data = iData; }
        if (data != null)
        {
            img.sprite = data.sprite;
            img.color = data.tint;
        }
        amt = amount;
    }
    #endregion

}

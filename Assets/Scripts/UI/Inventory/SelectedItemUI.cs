using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Selected item display 
/// </summary>
public class SelectedItemUI : MonoBehaviour
{
    #region Declerations
    /// <summary>Count display </summary>
    public TextMeshProUGUI countDisplay;
    /// <summary>Item image </summary>
    public Image itemSlot;
    /// <summary>Inventory reference </summary>
    public Inventory inventory => GameManager.instance.inventory;
    #endregion

    #region MonoBehaveiuors
    void Awake()
    {
        countDisplay = GetComponentInChildren<TextMeshProUGUI>();
    }
    #endregion

    #region Shift Items
    /// <summary>
    /// Shifts the items up 1
    /// </summary>
    public void ShiftUp()
    {
        HUDManager.instance.inventoryUI.SetSelectedItem(inventory.selcetedIndex + 1);
        RefreshDisplay();
    }
    /// <summary>
    /// Shifts the items down 1
    /// </summary>
    public void ShiftDown()
    {
        HUDManager.instance.inventoryUI.SetSelectedItem(inventory.selcetedIndex - 1);
        RefreshDisplay();
    
    }
    #endregion

    #region refresh display
    /// <summary>
    /// Refereshs the display to shoiw the current values
    /// </summary>
    public void RefreshDisplay()
    {
        string itemName = GameManager.instance.inventory.GetItemFromIndex(inventory.selcetedIndex);
        ItemData itemData = HUDManager.instance.inventoryUI.itemDataReference.GetItemData(itemName);
        if (itemData != null)
        {
            itemSlot.sprite = itemData.sprite;
            itemSlot.color = itemData.tint;
            countDisplay.text = "X" + inventory.contents[inventory.selcetedIndex].Value;
        }
        else
        {
            itemSlot.sprite = null;
            itemSlot.color = Color.clear;
            countDisplay.text = "X" + 0;
        }
    }
    #endregion
}

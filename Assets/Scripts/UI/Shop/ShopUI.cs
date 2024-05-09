using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;



/// <summary>
/// A button for the shop menu.
/// </summary>
public class ShopUI : MonoBehaviour
{
    /// <summary>
    /// The text component that displays the name of this item's button.
    /// </summary>
    [SerializeField] TextMeshProUGUI nameText;
    /// <summary>
    /// The text component that displays the quantity count of this button's item.
    /// </summary>
    [SerializeField] TextMeshProUGUI quantityText;
    /// <summary>
    /// The text component that displays the cost of this button's item.
    /// </summary>
    [SerializeField] TextMeshProUGUI costText;

    /// <summary>
    /// The 'SaleItem' struct that this button represents.
    /// </summary>
    SaleItem saleItem => parentShop.inventory[itemIndex];
    /// <summary>
    /// The shop that holds the item this UI element represents.
    /// </summary>
    Shop parentShop = null;
    /// <summary>
    /// The shop's inventory index that contains the item this UI element represents.
    /// </summary>
    int itemIndex = -1;

    /// <summary>
    /// Updates the button's text and other visual aspects.
    /// </summary>
    /// <param name="newShop"></param>
    /// <param name="index"></param>
    public void UpdateButton(Shop newShop = null, int index = -1)
    {
        //Cache reference to the shop that currently owns this button
        if (newShop != null)
        { parentShop = newShop; }
        if (index != -1)
        { itemIndex = index; }

        //Set button text
        if (parentShop != null && itemIndex > -1)
        {
            nameText.text = saleItem.itemName;
            costText.text = saleItem.cost.ToString();
            if (saleItem.quantity >= 0)
            {
                quantityText.gameObject.SetActive(true);
                quantityText.text = saleItem.quantity.ToString();
            }
            else
            {
                quantityText.gameObject.SetActive(false);
            }
        }

        //Enable/disable button
        if (saleItem.quantity != 0)
        {
            Enable();
        }
        else
        {
            Disable();
        }
    }

    /// <summary>
    /// Purchases the item that this UI element represents.
    /// </summary>
    public void PurchaseItem()
    {
        if (GameManager.instance.goldAmount >= saleItem.cost)
        {
            //Decrease quantity
            if (saleItem.quantity > 0)
            { parentShop.inventory[itemIndex].quantity--; }
            //Update button
            UpdateButton();
            //Remove player's gold
            GameManager.instance.RemoveGold(saleItem.cost);
            //Add item
            if (ItemPool.instance.itemReferences.GetItemData(saleItem.itemName).isInventory)
            { GameManager.instance.inventory.AddToInventory(saleItem.itemName, 1); }
        }
    }


    /// <summary>
    /// Enables this button.
    /// </summary>
    private void Enable()
    {
        GetComponent<Button>().interactable = true;
    }
    /// <summary>
    /// Disables this button.
    /// </summary>
    public void Disable()
    {
        GetComponent<Button>().interactable = false;
        //Set selected button
        EventSystem.current.SetSelectedGameObject(parentShop.xButton);

        //Get all needed references for rewiring navigation
        Navigation navigation = GetComponent<Button>().navigation;
        Navigation upNav = navigation;
        Navigation downNav = navigation;
        Button upButton = null;
        Button downButton = null;
        if (navigation.selectOnUp != null)
        {
            upButton = navigation.selectOnUp.GetComponent<Button>();
            upNav = upButton.navigation;
        }
        if (navigation.selectOnDown != null)
        {
            downButton = navigation.selectOnDown.GetComponent<Button>();
            downNav = downButton.navigation;
        }
        //Rewire navigation
        if (downButton != null)
        {
            //Change navigation
            upNav.selectOnDown = downButton;
            downNav.selectOnUp = upButton;
        }
        else
        {
            upNav.selectOnDown = null;
        }
        //Assign upNav
        if (upButton != null)
        { upButton.navigation = upNav; }
        //Assign downNav
        if (downButton != null)
        { downButton.navigation = downNav; }
        //Clear self navigation
        navigation.selectOnUp = null;
        navigation.selectOnDown = null;
        GetComponent<Button>().navigation = navigation;
    }
}

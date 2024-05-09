using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The 'X' button used in the shop AI.
/// </summary>

public class ShopXUI : MonoBehaviour
{
    /// <summary>
    /// The shop object that this UI element belongs to.
    /// </summary>
    [HideInInspector] public Shop shop = null;

    /// <summary>
    /// Tells the shop to close.
    /// </summary>
    public void CloseShop()
    {
        if (shop && shop.open)
        {
            shop.CloseShop();
        }
    }
}

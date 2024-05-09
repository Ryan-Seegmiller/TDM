using FloatingText;
using System;
using UnityEngine;

[Serializable]
public class ItemData
{
    [Header("Data To Use")]
    ///Identifier
    public string itemName = "";
    ///Details for editor
    [TextArea]public string itemDescription;
    ///Reference to Item
    public Item itemObject;

    [Header("Inventory Section")]
    public Color tint;
    public Sprite sprite;
    //public Mesh mesh; //For 3D models
    public int maxHoldableAmount = 10;
    ///Determines whether or not it can go into the inventory
    public bool isInventory = true;


    [Header("Pickup text")]
    ///Pop-up text on pickup
    public FloatingTextValues pickupText;

    /// <summary> Enable Pop-up text </summary>
    /// <param name="position"></param>
    public void ShowPickupText(Vector3 position)
    {
        FloatingTextManager.instance.SetStationaryFloatingText(pickupText, position);
    }

}

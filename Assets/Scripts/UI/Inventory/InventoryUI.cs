using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    #region Declerations
    [Header("Inventory slots"), Tooltip("Must be in order")]
    ///<summary>Inventory UI slots</summary>
    [NonReorderable] public InventorySlot[] inventorySlots = new InventorySlot[10];

    [Header("Mouse Image")]
    ///<summary>Image that the mouse</summary>
    public Image mouseImage;

    [Header("Toggle")]
    ///<summary>arrow transform</summary>
    public RectTransform dropdownArrowTransform;

    #region Inventory
    ///<summary>is the inventory active or not</summary>
    [NonSerialized]public bool inventoryActive = false;
    ///<summary>Is there an item attached to the mouse</summary>
    private bool itemAttached = false;
    ///<summary>Previous index of the item</summary>
    private int previousIndex;
    ///<summary>The data that is attached the mouse if there is data</summary>
    private KeyValuePair<string, int> attachedData;
    [Header("Item References")]
    ///<summary>Item data storage</summary>
    public ItemDataStorage itemDataReference;

    //Inventory Ref
    private Inventory inventory => GameManager.instance.inventory;
    #endregion
    #endregion

    #region Unity Stuff
    ///<summary>Graphics raycaster reference</summary>
    private GraphicRaycaster m_Raycaster;
    ///<summary>Pointer event data reference</summary>
    private PointerEventData m_PointerEventData;
    ///<summary>EventSystem reference</summary>
    private EventSystem m_EventSystem;
    ///<summary>Canvas group reference</summary>
    private CanvasGroup cvGroup;

    private void Start()
    {
        m_Raycaster = GetComponentInParent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();
        cvGroup = GetComponent<CanvasGroup>();
        cvGroup.alpha = 0;
    }

    private void Update()
    {
        if (!inventoryActive) { return; }
        MoveSprite();
    }
    #endregion

    #region Toggle Inventory
    /// <summary>
    /// Toggles the inventory on and off
    /// </summary>
    public void ToggleInvetory()
    {
        if (Application.isMobilePlatform)
        {
            MobileControlManager.instance.ToggleControls();
        }

        if (inventoryActive)
        {
            DeactivateInventory();
        }
        else
        {
            ActivateInvetory();
        }
       
    }
    /// <summary>
    /// Daeactivated=s the inventory display
    /// </summary>
    public void DeactivateInventory()
    {   //Make the inventory not show
        cvGroup.alpha = 0;
        //If there is an attached image it will put the item back into the inventory either in the skot it was previously at or the next open slot
        if (itemAttached)
        {   //Previous slot
            if (!inventorySlots[previousIndex].HasImage()) 
            {
                PlaceImage(attachedData.Key, previousIndex);
            }
            else
            {
                //Next open slot
                for (int i = 0; i < inventorySlots.Length; i++)
                {
                    if (!inventorySlots[i].HasImage())
                    {
                        PlaceImage(attachedData.Key, i);
                        break;
                    }
                }
            }
        }
        inventoryActive = false;
        //Lock mouse
        CameraController.instance.LockMouse(true);
    }
    /// <summary>
    /// Turns the inventory back on
    /// </summary>
    private void ActivateInvetory()
    {
        cvGroup.alpha = 1;
        inventoryActive = true;
        SetSelectedItem(inventory.selcetedIndex);
        //Unlock mouse
        CameraController.instance.LockMouse(false);
    }
    #endregion

    #region Mouse
    /// <summary>
    /// Attaches the selected sprite to the mouse movement
    /// </summary>
    private void MoveSprite()
    {   //Returns early if no sprite attached
        if (!itemAttached) { return; }
        mouseImage.rectTransform.position = Input.mousePosition;
    }
    /// <summary>
    /// Attaches or deatches an item to the mouse
    /// </summary>
    public void OnLeftClick()
    {
        if (!inventoryActive) { return; }
        List<RaycastResult> results = GetRayacstResults();

        int index = SearchRaycast(results);
        if (index == -1)
        {
            if (itemAttached)
            {
                ItemPool.instance.SpawnItem(GameManager.instance.player.transform.position, attachedData.Key, attachedData.Value);
                ResetMouseImage();
            }
            return;
        }
        previousIndex = index;
        if (itemAttached)
        {
            PlaceImage(attachedData.Key, index);
        }
        else
        {
            AttachImage(index);
        }
        RefreshSelectedSlot();
    }
    /// <summary>
    /// Sets the selcted spot in the inventory
    /// </summary>
    public void OnRightCLick()
    {
        if (!inventoryActive) { return; }
        List<RaycastResult> results = GetRayacstResults();
        int index = SearchRaycast(results);
        if (index == -1) { return; }
        previousIndex = index;

        SetSelectedItem(index);
    }
    public void SetSelected(InventorySlot slot)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if(slot == inventorySlots[i])
            {
                SetSelectedItem(i);
                return;
            }
        }
    }
   
    public void ResetMouseImage()
    {
        //Reset item attached values
        itemAttached = false;
        mouseImage.color = Color.clear;
        attachedData = default;
    }
    #endregion

    #region Raycast
    /// <summary>
    /// Gets the raycast reuslt
    /// </summary>
    /// <returns></returns>
    private List<RaycastResult> GetRayacstResults()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        m_Raycaster.Raycast(m_PointerEventData, results);
        return results;
    }
    /// <summary>
    /// Searches the raycast
    /// </summary>
    /// <param name="results"></param>
    /// <returns></returns>
    private int SearchRaycast(List<RaycastResult> results)
    {
        foreach (RaycastResult result in results)
        {
            InventorySlot inventorySlot = result.gameObject.GetComponent<InventorySlot>();
            if (inventorySlot == null) { continue; }
            
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i] == inventorySlot)
                {
                    return i;
                }
            }
        }
        return -1;
    }
    #endregion

    #region Inventory behaveiuor
    /// <summary>
    /// Sets the item clicked on top the selected item
    /// </summary>
    /// <param name="results"></param>
    public void SetSelectedItem(int index)
    {   //Deselcted the current image
        inventorySlots[inventory.selcetedIndex].Deselect();
        inventory.selcetedIndex = index;

        RefreshSelectedSlot();
        HUDManager.instance.selectedUI.RefreshDisplay();//Refreshes the diplay
    }
    #endregion

    #region Image Behaviours
    /// <summary>
    /// Places an image using the the ID of the item and the index ofd the item that was placed
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="index"></param>
    public void PlaceImage(string itemID, int index)
    {
        //Gets the item data from the string inputed
        ItemData data = itemDataReference.GetItemData(itemID);
        if(data == null) { return; }//If there was no assciated item data it returns

        InventorySlot inventorySlotToUse = inventorySlots[index];

        if (itemAttached)
        {
            //Adds items to that slot if its the same item
            if (inventory.contents[index].Key == attachedData.Key)
            {
                int newAmount = inventory.contents[index].Value + attachedData.Value;
                if (newAmount > data.maxHoldableAmount)
                {
                    inventory.contents[index] = new KeyValuePair<string, int>(attachedData.Key, data.maxHoldableAmount);
                    attachedData = new KeyValuePair<string, int>(attachedData.Key, newAmount - data.maxHoldableAmount);
                }
            }
            //Places the item on the slot if its the same item
            else if(inventory.contents[index].Key == null)
            {
                inventory.PlaceAtIndex(attachedData, index);
                ResetMouseImage();
            }
            //Replaces the item on the tile if there is nothing there
            else if (inventory.contents[index].Key != null)
            {
                KeyValuePair<string, int> tempData = new KeyValuePair<string, int>(attachedData.Key, attachedData.Value);
                AttachImage(index);
                inventory.PlaceAtIndex(tempData, index);
            }
            
        }

        inventorySlotToUse.SetImage(data.sprite, data.tint);
        inventorySlotToUse.SetCount(inventory.GetAmount(index));
        HUDManager.instance.selectedUI.RefreshDisplay();
    }

    /// <summary>
    /// Removes an image at slot
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public ItemData RemoveImage(int index)
    {
        //This pulls the item data using the key from the stored id inside the inventory
        ItemData dataAtIndex = itemDataReference.GetItemData(inventory.contents[index].Key);

        inventorySlots[index].RemoveImage();

        inventorySlots[index].SetCount(0);

        HUDManager.instance?.selectedUI.RefreshDisplay();
        return dataAtIndex;
    }
    /// <summary>
    /// Selects the image
    /// </summary>
    /// <param name="index"></param>
    public void AttachImage(int index)
    {
        //Gets the sprite asociated
        ItemData attachedData = RemoveImage(index);
        if(attachedData == null) { return; }

        //Resets the attached values
        mouseImage.color = attachedData.tint;
        mouseImage.sprite = attachedData.sprite;
        this.attachedData = inventory.RemoveAtIndex(index);//Sets the attached data to the data from the index taht was removed

        itemAttached = true;
    }
   
    /// <summary>
    /// Refreshes the selected Slot
    /// </summary>
    public void RefreshSelectedSlot()
    {
        inventorySlots[inventory.selcetedIndex].Select();
        HUDManager.instance.selectedUI.RefreshDisplay();
    }
    #endregion

    #region Controller
    /// <summary>
    /// Scrolls the inventory from the controller input
    /// </summary>
    /// <param name="index"></param>
    public void ScrollSelectedIndex(int index)
    {
        if(index + inventory.selcetedIndex >= inventorySlots.Length || index + inventory.selcetedIndex < 0) { return; }
        index += inventory.selcetedIndex;
        int newIndex = Mathf.Clamp(index, 0, inventorySlots.Length - 1);
       
        SetSelectedItem(newIndex);
    }
    #endregion
}

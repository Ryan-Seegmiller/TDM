using System.Collections.Generic;
/// <summary>
/// Stores a collection of data in the form of an inventory
/// </summary>
public class Inventory
{
    #region Declerations
    /// <summary>
    /// size of the inventory
    /// </summary>
    public int invetorySize;
    /// <summary>
    /// Wether or not the inventory is full
    /// </summary>
    public bool inventoryFull = false;
    /// <summary>
    /// The actual key value pairs that is stored in the inventory
    /// </summary>
    public KeyValuePair<string, int>[] contents = new KeyValuePair<string, int>[20];
    #endregion

    #region Properties
    private int _selectedIndex = 0;
    /// <summary>
    /// The index that is the current displayed/Selcteed in the inventory
    /// </summary>
    public int selcetedIndex
    {
        get { return _selectedIndex; }
        set
        {
            //Keeps the selceted index from going out of bounds
            if(value >= invetorySize)
            {
                value -= invetorySize;
            }
            else if(value < 0)
            {
                value += invetorySize;
            }
            _selectedIndex = value;
        }
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Dertermines the size and max storage amount for each item
    /// </summary>
    /// <param name="invetorySize"></param>
    /// <param name="maxStorageAmount"></param>
    public Inventory(int invetorySize)
    {
        this.invetorySize = invetorySize;
        contents = new KeyValuePair<string, int>[invetorySize];
    }
    #endregion

    #region Retrieval
    /// <summary>
    /// Removes from inventory from an item and its amount
    /// </summary>
    /// <param name="data"></param>
    /// <param name="amount"></param>
    public void RemoveFromInvetory(string itemID, int amount = 1)
    {
        ItemData data = ItemPool.instance.itemReferences.GetItemData(itemID);
        for (int i = 0; i < invetorySize; i++)
        {
            if (contents[i].Key == null) { continue; }

            //If the item is found add to that item
            if (data != null && contents[i].Key == data.itemName)
            {
                //if the item overflows it sets the amount to the overflow value
                if (contents[i].Value - amount <= 0)
                {
                    contents[i] = new KeyValuePair<string, int>(null, 0);
                    HUDManager.instance.inventoryUI.inventorySlots[i].RemoveImage();
                    HUDManager.instance.inventoryUI.inventorySlots[i].SetCount(contents[i].Value);
                    continue;
                }
                contents[i] = new KeyValuePair<string, int>(contents[i].Key, contents[i].Value - amount);
                HUDManager.instance.inventoryUI.inventorySlots[i].SetCount(contents[i].Value);
                if (contents[i].Value <= 0)
                {
                    HUDManager.instance.inventoryUI.inventorySlots[i].RemoveImage();
                    HUDManager.instance.inventoryUI.inventorySlots[i].SetCount(contents[i].Value);
                }
                break;
            }
        }
    }
    /// <summary>
    /// Gets the item name from an index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public string GetItemFromIndex(int index)
    {
        return contents[index].Key;
    }
    /// <summary>
    /// Finds the first empty slot in inventory
    /// </summary>
    /// <returns></returns>
    public int FindFirstEmpty()
    {
        int firstEmpty = -1;
        for (int i = 0; i < invetorySize; i++)
        {
            if (contents[i].Key == null)
            {
                firstEmpty = i;
                break;
            }
        }
        return firstEmpty;
    }
    /// <summary>
    /// Retirns the index at which the item is in the inventory
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="maxAmount"></param>
    /// <returns></returns>
    public int FindItemInInventory(string itemID, int maxAmount)
    {
        for (int i = 0; i < invetorySize; i++)
        {
            if (contents[i].Key == itemID)
            {
                if (contents[i].Value >= maxAmount)
                {
                    continue;
                }
                return i;
            }
        }
        return -1;
    }
  
    /// <summary>
    /// Removes and returns the item at the index provided
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public KeyValuePair<string, int> RemoveAtIndex(int index)
    {
        KeyValuePair<string, int> dataAtIndex = new KeyValuePair<string, int>( contents[index].Key, contents[index].Value);
        contents[index] = default;
        return dataAtIndex;
    }
    /// <summary>
    /// Gets the amount of an item in the specified spot
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int GetAmount(int index)
    {
        return contents[index].Value;
    }
    /// <summary>
    /// Uses the selected item
    /// </summary>
    /// <param name="amount"></param>
    public void UseSelectedItem(int amount = 1)
    {
        string itemID = contents[selcetedIndex].Key;
        if (itemID != null)
        {
            if (ItemPool.instance.Retrive(itemID).Use())
            {
                contents[selcetedIndex] = new KeyValuePair<string, int>(itemID, contents[selcetedIndex].Value - amount);
                if (contents[selcetedIndex].Value <= 0)
                {
                    contents[selcetedIndex] = default;
                    HUDManager.instance.inventoryUI.inventorySlots[selcetedIndex].RemoveImage();
                }
                HUDManager.instance.inventoryUI.inventorySlots[selcetedIndex].SetCount(contents[selcetedIndex].Value);
                HUDManager.instance.inventoryUI.RefreshSelectedSlot();
            }
        }
    }
    /// <summary>
    /// Retunrs wether or not there is a item equiped
    /// </summary>
    /// <returns></returns>
    public bool HasItemEquiped()
    {
        return contents[selcetedIndex].Key != null;
    }
    #endregion

    #region Placement
    /// <summary>
    /// Adds to the inventory from an item and an amount
    /// </summary>
    /// <param name="data"></param>
    /// <param name="amount"></param>
    public void AddToInventory(string itemID, int amount = 1)
    {
        ItemData data = ItemPool.instance.itemReferences.GetItemData(itemID);

        if (inventoryFull) { return; }
        while (amount > 0)
        {
            int itemIndex = FindItemInInventory(data.itemName, data.maxHoldableAmount);
            if (itemIndex == -1)
            {
                itemIndex = FindFirstEmpty();
            }
            if (itemIndex == -1) { inventoryFull = true; break; }
            if ((contents[itemIndex].Value + amount) > data.maxHoldableAmount)
            {
                int overflow = (contents[itemIndex].Value + amount) - data.maxHoldableAmount;
                contents[itemIndex] = new KeyValuePair<string, int>(data.itemName, contents[itemIndex].Value + (amount - overflow));
                amount = overflow;
                //UI
                HUDManager.instance.inventoryUI.PlaceImage(data.itemName, itemIndex);
                HUDManager.instance.selectedUI.RefreshDisplay();
            }
            else if ((contents[itemIndex].Value + amount) <= data.maxHoldableAmount)
            {
                contents[itemIndex] = new KeyValuePair<string, int>(data.itemName, contents[itemIndex].Value + amount);
                amount = 0;
                HUDManager.instance.inventoryUI.PlaceImage(data.itemName, itemIndex);
                HUDManager.instance.selectedUI.RefreshDisplay();
            }
            else
            {
                inventoryFull = true;
                break;
            }
        }
    }
    /// <summary>
    /// Places an item with an amount at an indexd position in the inventory array
    /// </summary>
    /// <param name="data"></param>
    /// <param name="index"></param>
    public void PlaceAtIndex(KeyValuePair<string, int> data, int index)
    {
        contents[index] = data;
    }
    #endregion
}

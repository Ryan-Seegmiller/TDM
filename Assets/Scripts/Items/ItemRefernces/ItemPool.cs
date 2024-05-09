using FloatingText;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : MonoBehaviour
{
    ///Singleton pattern
    public static ItemPool instance { get; private set; }

    ///Items that exist physically
    public List<Item> inWorldItems = new List<Item>();
    ///Items stored in the object pool
    public List<Item> pooledItems = new List<Item>();
    ///References to item types
    public ItemDataStorage itemReferences;

    //public Mesh stackMesh;
    ///Sprite for multiple items in one stack for referencing
    public Sprite stackSprite;
    ///Floating text values for multiple items in one stack for referencing
    public FloatingTextValues stackTextValues;

    void Awake()
    {
        //Singleton
        if(instance == null) { instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        Generate(20);
    }

    /// <summary> Spawns a set amount of each item type to store in the pool </summary>
    /// <param name="amountOfEach"></param>
    public void Generate(int amountOfEach)
    {
        foreach (ItemData item in itemReferences.itemsData)
        {
            if (item != null)
            {
                for (int i = 0; i < amountOfEach; i++)
                {
                    Item itemObject = Instantiate(item.itemObject, transform);
                    itemObject.isInventory = item.isInventory;
                    itemObject.SetUp(item.itemName);
                    itemObject.pickup += () => { item.ShowPickupText(itemObject.transform.position); };
                }
            }
        }
    }

    /// <summary> Put an item into storage </summary>
    /// <param name="item"></param>
    public void Store(Item item)
    {
        pooledItems.Add(item);
        inWorldItems.Remove(item);
    }
    /// <summary> Get reference to an item in storage </summary>
    /// <param name="itemID"></param>
    public Item Retrive(string itemID)
    {
        Item result = null;

        foreach (Item item in pooledItems)
        {
            if (item.itemID == itemID)
            {
                result = item;
                break;
            }
        }

        return result;
    }
    /// <summary> Proximity check for items at a location </summary>
    /// <param name="checkingPosition"></param>
    /// <returns> Any nearby item </returns>
    public Item GetAtPosition(Vector3 checkingPosition)
    {
        foreach(Item item in inWorldItems)
        {
            if (!item.gameObject.activeSelf) { continue; }
            if(Vector3.Distance(item.transform.position, checkingPosition) <= 1f )
            {
                return item;
            }
        }

        return null;
    }

    /// <summary> Spawn an item at a location by ID </summary>
    /// <param name="spawnPos"></param>
    /// <param name="itemID"></param>
    /// <param name="amount"></param>
    public void SpawnItem(Vector3 spawnPos, string itemID, int amount = 1)
    {
        foreach(Item item in pooledItems)
        {
            if(item.itemID == itemID)
            {
                item.Spawn(spawnPos, amount);
                return;
            }
        }
    }
    /// <summary> Spawns the first available item at the location </summary>
    /// <param name="vec"></param>
    public void SpawnRandomAt(Vector3 vec)
    {
        if (pooledItems.Count > 0)
        {
            pooledItems[0].Spawn(vec, 1);
        }
    }
    /// <summary> Returns item data by name </summary>
    /// <param name="name"></param>
    public ItemData GetDataOf(string name)
    {
        return itemReferences.GetItemData(name);
    }
}

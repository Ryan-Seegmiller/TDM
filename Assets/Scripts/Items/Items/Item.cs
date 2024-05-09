using FloatingText;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    ///Identifier for findability
    [NonSerialized]public string itemID;

    //protected MeshFilter meshFIlter;
    ///Displaying sprite in-world
    protected SpriteRenderer spriteRend; //for temporary sprite display 3D
    public delegate void OnPickup();
    public OnPickup pickup;

    public int amount = 1;
    ///Determines whether or not it can go into the inventory
    [HideInInspector]public bool isInventory = true;

    //Ryan no touchy
    bool is_Stack = false;
    ///Handles visual and functional differences between representing single and multiple items
    public bool isStack
    {
        get { return is_Stack; }
        set
        {
            is_Stack = value;
            if (is_Stack)
            {
                //meshFIlter.mesh = ItemPool.instance.stackMesh;
                //for 3D sprite
                spriteRend.sprite = ItemPool.instance.stackSprite;
                spriteRend.color = Color.white;
                stackData.Add(new KeyValuePair<string, int>(itemID, amount));
            }
            else
            {
                //meshFIlter.mesh = ItemPool.instance.GetDataOf(itemID).mesh;
                //for 3D sprite
                spriteRend.sprite = ItemPool.instance.GetDataOf(itemID).sprite;
                spriteRend.color = ItemPool.instance.GetDataOf(itemID).tint;
                stackData.Clear();
            }
        }
    }
    ///Stores what's in the stack
    public List<KeyValuePair<string, int>> stackData = new List<KeyValuePair<string, int>>();

    protected void Start()
    {
       ReturnToPool(); 
    }

    /// <summary> Assigns ID and visual components </summary>
    /// <param name="itemID">The ID</param>
    /// <param name="amount">How many</param>
    public void SetUp(string itemID,int amount = 1)
    {
        //if (meshFIlter == null) { meshFIlter = GetComponent<MeshFilter>(); }
        if(spriteRend == null) { spriteRend = GetComponent<SpriteRenderer>(); }
        spriteRend.sprite = ItemPool.instance.GetDataOf(itemID).sprite;
        spriteRend.color = ItemPool.instance.GetDataOf(itemID).tint;
        this.itemID = itemID;
        this.amount = amount;
    }

    ///Adds to inventory and returns to object pool
    public virtual void Pickup()
    {
        if (isStack)
        {
            for (int i = 0; i < stackData.Count; i++)
            {
                if (ItemPool.instance.itemReferences.GetItemData(stackData[i].Key).isInventory)
                {
                    GameManager.instance.inventory.AddToInventory(stackData[i].Key, stackData[i].Value);
                }
            }
            isStack = false;
            FloatingTextManager.instance.SetStationaryFloatingText(ItemPool.instance.stackTextValues, transform.position);
            ReturnToPool();
            return;
        }
        if (isInventory)
        {
            GameManager.instance.inventory.AddToInventory(itemID , amount);
        }
        pickup?.Invoke();
        ReturnToPool();
    }

    /// <summary> Remove from inventory and place on the ground </summary>
    /// <param name="position"></param>
    /// <param name="amount"></param>
    public virtual void Drop(Vector3 position, int amount)
    {
        Spawn(position, amount);
    }

    ///Default, implementation-specific behaviors
    public virtual bool Use() { return false; }

    /// <summary> Take item(s) from the object pool and spawn at a location </summary>
    /// <param name="position"></param>
    /// <param name="amount"></param>
    public void Spawn(Vector3 position, int amount)
    {
        this.amount = amount;
       
        ItemPool.instance.inWorldItems.Add(this);
        ItemPool.instance.pooledItems.Remove(this);

        Item nearby = ItemPool.instance.GetAtPosition(position);

        if(nearby?.itemID == this.itemID)//Makes sure there no items of the same type nearby
        {
            //Tacks the items if can
            nearby.amount += amount;
            ReturnToPool();
            return;
        }
        else if (nearby != null)
        {
            if (nearby.isStack)
            {
                
                for(int i = 0; i < nearby.stackData.Count; i++)
                {
                    if (nearby.stackData[i].Key == this.itemID)
                    {
                        nearby.stackData[i] = new KeyValuePair<string, int>(itemID, nearby.stackData[i].Value + amount);
                        break;
                    }
                }
            }
            else
            {
                nearby.isStack = true;
                nearby.stackData.Add(new KeyValuePair<string, int>(itemID, amount));
            }
            ReturnToPool();
            return;
        }

        //Places the item
        gameObject.SetActive(true);
        transform.position = position;

    }

    ///Remove from the world and return to the object pool
    public void ReturnToPool()
    {
        ItemPool.instance.Store(this);
        gameObject.SetActive(false);
    }
}

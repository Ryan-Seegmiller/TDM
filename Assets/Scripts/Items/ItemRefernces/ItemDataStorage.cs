using UnityEngine;

[CreateAssetMenu(fileName = "Item Data", menuName = "GameData/Items/Items")]
public class ItemDataStorage : ScriptableObject
{
    ///Stores every type of item
    public ItemData[] itemsData;

    /// <summary> Retrives a reference to an item's data by name </summary>
    /// <param name="name"></param>
    public ItemData GetItemData(string name)
    {
        if(name == null) { return null; }
        for (int i = 0; i < itemsData.Length; i++)
        {
            if (itemsData[i].itemName.ToLower() == name.ToLower())
            {
                return itemsData[i];
            }
        }
        return null;
    }
}

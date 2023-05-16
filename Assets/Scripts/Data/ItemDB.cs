using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB
{
    static Dictionary<string, ItemBase> items;

    public static void Init()
    {
        items = new Dictionary<string, ItemBase>();

        var itemList = Resources.LoadAll<ItemBase>("Prefabs/Items");
        foreach (var item in itemList)
        {
            if (items.ContainsKey(item.ItemName))
            {
                Debug.LogError($"There are 2 items with the same name {item.ItemName}");
                continue;
            }

            items[item.ItemName] = item;
        }
    }

    public static ItemBase GetItemByName(string name)
    {
        if (!items.ContainsKey(name))
        {
            Debug.LogError($"Item not found with the name {name}");
            return null;
        }

        return items[name];
    }
}

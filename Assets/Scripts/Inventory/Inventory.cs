using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCategory { Items, Pokeballs, Tms, Matetials, KeyItems }

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemSlot> slots;
    [SerializeField] private List<ItemSlot> pokeballSlots;
    [SerializeField] private List<ItemSlot> tmSlot;
    [SerializeField] private List<ItemSlot> materialSlots;
    [SerializeField] private List<ItemSlot> keyItemSlots;


    private List<List<ItemSlot>> allSlots;

    public event Action OnUpdated;

    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "消耗品", "精灵球", "技能学习器", "材料", "重要道具"
    };

    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>() { slots, pokeballSlots, tmSlot, materialSlots, keyItemSlots };
    }

    public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }
    
    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    public ItemBase GetItem(int itemIndex, int categoryIndex)
    {
        var currentSlots = GetSlotsByCategory(categoryIndex);
        return currentSlots[itemIndex].Item;
    }

    public ItemBase UseItem(int itemIndex, Pokemon selectedPokemon, int selectedCategory)
    {
        var item = GetItem(itemIndex, selectedCategory);
        bool itemUsed = item.Use(selectedPokemon);
        if (itemUsed)
        {
            RemoveItem(item, selectedCategory);
            return item;
        }

        return null;
    }

    public void AddItem(ItemBase item, int count=1)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);
        if (itemSlot != null)
        {
            itemSlot.Count += count;
        }
        else
        {
            currentSlots.Add(new ItemSlot()
            {
                Item = item,
                Count = count
            });
        }

        OnUpdated?.Invoke();
    }

    private ItemCategory GetCategoryFromItem(ItemBase item)
    {
        if (item is RecoveryItem)
        {
            return ItemCategory.Items;
        }
        else if (item is PokeballItem)
        {
            return ItemCategory.Pokeballs;
        }
        else if (item is TmItem)
        {
            return ItemCategory.Tms;
        }
        else if (item is null) 
        {
            return ItemCategory.Matetials;
        }

        return ItemCategory.KeyItems;
    }

    public void RemoveItem(ItemBase item, int selectedCategory) 
    {
        var currentSlots = GetSlotsByCategory(selectedCategory);

        var itemSlot = currentSlots.First(slot => slot.Item == item);
        itemSlot.Count--;
        if (itemSlot.Count == 0)
        {
            currentSlots.Remove(itemSlot);
        }

        OnUpdated?.Invoke();
    }
}

[Serializable]
public class ItemSlot
{
    [SerializeField] private ItemBase item;
    [SerializeField] private int count;

    public ItemBase Item {
        get => item;
        set => item = value;
    }
    public int Count {
        get => count;
        set => count = value;
    }
}

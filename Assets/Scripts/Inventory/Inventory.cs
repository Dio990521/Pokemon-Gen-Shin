using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCategory { Items, Pokeballs, Tms, Misc, KeyItems }

public class Inventory : MonoBehaviour, ISavable
{
    [SerializeField] private List<ItemSlot> itemSlots;
    [SerializeField] private List<ItemSlot> pokeballSlots;
    [SerializeField] private List<ItemSlot> tmSlots;
    [SerializeField] private List<ItemSlot> miscSlots;
    [SerializeField] private List<ItemSlot> keyItemSlots;

    private List<List<ItemSlot>> allSlots;

    public event Action OnUpdated;

    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "消耗品", "精灵球", "技能学习器", "杂物", "重要道具"
    };

    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>() { itemSlots, pokeballSlots, tmSlots, miscSlots, keyItemSlots };
    }

    public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }
   
    public static Inventory GetInventory()
    {
        return GameManager.Instance.PlayerController.GetComponent<Inventory>();
    }

    public ItemBase GetItem(int itemIndex, int categoryIndex)
    {
        var currentSlots = GetSlotsByCategory(categoryIndex);
        if (currentSlots.Count > 0)
        {
            return currentSlots[itemIndex].Item;
        }
        return null;
    }

    public ItemBase UseItem(int itemIndex, Pokemon selectedPokemon, int selectedCategory)
    {
        var item = GetItem(itemIndex, selectedCategory);
        return UseItem(item, selectedPokemon);
    }

    public ItemBase UseItem(ItemBase item, Pokemon selectedPokemon)
    {
        if (item == null)
        {
            return null;
        }
        bool itemUsed = item.Use(selectedPokemon);
        if (itemUsed)
        {
            RemoveItem(item);
            return item;
        }

        return null;
    }

    public void AddItem(ItemBase item, int count=1, bool playSE=true)
    {
        
        int category = (int)GetCategoryFromItem(item);
        if (playSE)
        {
            if (category == (int)ItemCategory.Tms)
            {
                AudioManager.Instance.PlaySE(SFX.OBTAIN_TM, true);
            }
            else if (item.IsBadge)
            {
                AudioManager.Instance.PlaySE(SFX.OBTAIN_BADGE, true);
            }
            else
            {
                AudioManager.Instance.PlaySE(SFX.OBTAIN_ITEM, true);
            }
        }

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
        if (item is RecoveryItem || item is EvolutionItem || item is BoostItem || item is AvoidPokemonItem || item is PaimengItem)
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
        else if (item is MiscItem) 
        {
            return ItemCategory.Misc;
        }

        return ItemCategory.KeyItems;
    }

    public bool HasItem(ItemBase item)
    {
        int selectedCategory = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(selectedCategory);

        return currentSlots.Exists(slot => slot.Item == item);
    }

    public int GetItemCount(ItemBase item)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);
    
        if (itemSlot != null)
        {
            return itemSlot.Count;
        }
        return 0;
    }

    public void RemoveItem(ItemBase item, int countToRemove=1) 
    {
        int selectedCategory = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(selectedCategory);

        var itemSlot = currentSlots.First(slot => slot.Item == item);
        itemSlot.Count -= countToRemove;
        if (itemSlot.Count == 0)
        {
            currentSlots.Remove(itemSlot);
        }

        OnUpdated?.Invoke();
    }

    public object CaptureState()
    {
        var saveData = new InventorySaveData()
        {
            items = itemSlots.Select(i => i.GetSaveData()).ToList(),
            pokeballs = pokeballSlots.Select(i => i.GetSaveData()).ToList(),
            tms = tmSlots.Select(i => i.GetSaveData()).ToList(),
            materials = miscSlots.Select(i => i.GetSaveData()).ToList(),
            keyItems = keyItemSlots.Select(i => i.GetSaveData()).ToList(),
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as InventorySaveData;

        itemSlots = saveData.items.Select(i => new ItemSlot(i)).ToList();
        pokeballSlots = saveData.pokeballs.Select(i => new ItemSlot(i)).ToList();
        tmSlots = saveData.tms.Select(i => new ItemSlot(i)).ToList();
        miscSlots = saveData.materials.Select(i => new ItemSlot(i)).ToList();
        keyItemSlots = saveData.keyItems.Select(i => new ItemSlot(i)).ToList();

        allSlots = new List<List<ItemSlot>>() { itemSlots, pokeballSlots, tmSlots, miscSlots, keyItemSlots };
        OnUpdated?.Invoke();
    }
}

[Serializable]
public class ItemSlot
{
    [SerializeField] private ItemBase item;
    [SerializeField] private int count;

    public ItemSlot()
    {

    }

    public ItemSlot(ItemSaveData saveData)
    {
        item = ItemDB.GetObjectByName(saveData.itemName); 
        count = saveData.count;
    }

    public ItemBase Item {
        get => item;
        set => item = value;
    }
    public int Count {
        get => count;
        set => count = value;
    }

    public ItemSaveData GetSaveData()
    {
        var saveData = new ItemSaveData()
        {
            itemName = item.name,
            count = Count
        };

        return saveData;
    }
}

[Serializable]
public class ItemSaveData
{
    public string itemName;
    public int count;
}

[Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> items;
    public List<ItemSaveData> pokeballs;
    public List<ItemSaveData> tms;
    public List<ItemSaveData> materials;
    public List<ItemSaveData> keyItems;

}

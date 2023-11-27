using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBase : ScriptableObject
{
    [SerializeField] private string itemName;

    [TextArea]
    [SerializeField] private string description;

    [SerializeField] private Sprite icon;
    [SerializeField] private int price;
    [SerializeField] private bool isSellable;
    [SerializeField] private int yuanshiPrice;

    public string ItemName => itemName;
    public string Description => description;
    public Sprite Icon => icon;
    public int Price => price;
    public bool IsSellable => isSellable;
    public int YuanshiPrice => yuanshiPrice;

    public bool IsBadge;

    public virtual bool Use(Pokemon pokemon)
    {
        return false;
    }

    public virtual bool CanUseInBattle => true;
    public virtual bool CanUseOutsideBattle => true;


}

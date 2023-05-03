using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text countText;

    public void SetData(ItemSlot itemSlot)
    {
        nameText.text = itemSlot.Item.ItemName;
        countText.text = $"x{itemSlot.Count}";
    }
}

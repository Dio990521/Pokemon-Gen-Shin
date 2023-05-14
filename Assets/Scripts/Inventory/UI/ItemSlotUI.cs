using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text countText;
    public GameObject cursorPos;

    public RectTransform rectTransform;

    public float Height => rectTransform.rect.height;

    private void Awake()
    {

    }

    public void SetData(ItemSlot itemSlot)
    {
        rectTransform = GetComponent<RectTransform>();
        nameText.text = itemSlot.Item.ItemName;
        countText.text = $"x{itemSlot.Count}";
    }
}

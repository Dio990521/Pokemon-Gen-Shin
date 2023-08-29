using PokeGenshinUtils.SelectionUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : SelectionUI<ItemSlotUI>
{
    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI itemSlotUI;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text itemDescription;

    [SerializeField] private Image upArrow;
    [SerializeField] private Image downArrow;

    private List<ItemBase> availableItems;
    private List<ItemSlotUI> slotUIList;
    private const int itemsInViewPort = 10;
    private RectTransform itemListRect;

    private void Awake()
    {
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    public void Show(List<ItemBase> availableItems)
    {
        selectedItem = 0;
        prevSelection = -1;
        this.availableItems = availableItems;
        gameObject.SetActive(true);
        UpdateItemList();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public override void HandleUpdate(bool allowCancel = true)
    {
        base.HandleUpdate();

        if (selectedItem != prevSelection)
        {
            UpdateSelectionUI();
        }
    }

    private void UpdateItemList()
    {
        // Clear all the existing items
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        foreach (var item in availableItems)
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetNameAndPrice(item);
            slotUIList.Add(slotUIObj);
        }

        SetItems(slotUIList);

        UpdateSelectionUI();
    }

    public override void UpdateSelectionUI()
    {
        base.UpdateSelectionUI();
        selectedItem = Mathf.Clamp(selectedItem, 0, availableItems.Count - 1);
        var item = availableItems[selectedItem];
        itemIcon.sprite = item.Icon;
        itemDescription.text = item.Description;

        HandleScrolling();
    }

    private void HandleScrolling()
    {

        bool showUpArrow = selectedItem > (itemsInViewPort / 2) && slotUIList.Count > itemsInViewPort;
        upArrow.gameObject.SetActive(showUpArrow);
        bool showDownArrow = selectedItem + (itemsInViewPort / 2) < slotUIList.Count && slotUIList.Count > itemsInViewPort;
        downArrow.gameObject.SetActive(showDownArrow);

        if (selectedItem + (itemsInViewPort / 2) <= slotUIList.Count)
        {
            float scrollPos = Mathf.Clamp(selectedItem - itemsInViewPort / 2, 0, selectedItem) * slotUIList[0].Height;
            itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);
        }
    }


}

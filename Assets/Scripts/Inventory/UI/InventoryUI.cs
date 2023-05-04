using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI itemSlotUI;

    [SerializeField] private Image itemIcon;
    [SerializeField] private Text itemDescription;

    [SerializeField] private float cursorXOffset;
    [SerializeField] private float cursorYOffset;

    [SerializeField] private Image upArrow;
    [SerializeField] private Image downArrow;

    private const int itemsInViewPort = 10;

    private Inventory inventory;
    private int selectedItem;
    private int prevSelection = -1;
    private List<ItemSlotUI> slotUIList;
    private RectTransform itemListRect;

    [SerializeField] private Image inventoryCursor;

    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateItemList();
    }
    private void UpdateItemList()
    {
        // Clear all the existing items
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();

        foreach (var itemSlot in inventory.Slots)
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);
            slotUIList.Add(slotUIObj);
        }
        UpdateUI();
    }

    public void HandleUpdate(Action onBack)
    {
        prevSelection = selectedItem;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++selectedItem;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            --selectedItem;
        }

        selectedItem = Mathf.Clamp(selectedItem, 0, inventory.Slots.Count - 1);

        if (prevSelection != selectedItem)
        {
            UpdateUI();
        }

        if (Input.GetKeyDown(KeyCode.X))
        { 
            onBack?.Invoke();
        }
    }

    private void UpdateUI()
    {
        
        var item = inventory.Slots[selectedItem].Item;
        itemIcon.sprite = item.Icon;
        itemDescription.text = item.Description;
        HandleScrolling();
    }

    private void HandleScrolling()
    {
        bool showUpArrow = selectedItem > (itemsInViewPort / 2);
        upArrow.gameObject.SetActive(showUpArrow);
        bool showDownArrow = selectedItem + (itemsInViewPort / 2) < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);

        if (!showUpArrow && !showDownArrow) 
        {
            inventoryCursor.rectTransform.position = slotUIList[selectedItem].rectTransform.position - new Vector3(cursorXOffset+8f, cursorYOffset, 0f);
        }
        
        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewPort / 2, 0, selectedItem) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

        if (showUpArrow || showDownArrow)
        {
            if (selectedItem > slotUIList.Count - (itemsInViewPort / 2))
            {
                inventoryCursor.rectTransform.position = slotUIList[selectedItem].rectTransform.position + new Vector3(0f, (slotUIList.Count - (itemsInViewPort / 2) - selectedItem) * slotUIList[0].Height, 0f) - new Vector3(cursorXOffset, cursorYOffset, 0f);
            }
            else
            {
                inventoryCursor.rectTransform.position = slotUIList[selectedItem].rectTransform.position - new Vector3(cursorXOffset, cursorYOffset, 0f);
            }
        }

    }

}

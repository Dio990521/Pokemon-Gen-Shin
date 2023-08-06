using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI itemSlotUI;
    [SerializeField] private Image inventoryCursor;

    [SerializeField] private Image itemIcon;
    [SerializeField] private Text itemDescription;

    [SerializeField] private Image upArrow;
    [SerializeField] private Image downArrow;

    private List<ItemBase> availableItems;
    private List<ItemSlotUI> slotUIList;
    private int selectedItem;
    private int prevSelection;
    private const int itemsInViewPort = 10;
    private RectTransform itemListRect;
    private Action<ItemBase> onItemSelected;
    private Action onBack;

    private void Awake()
    {
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    public void Show(List<ItemBase> availableItems, Action<ItemBase> onItemSelected,
        Action onBack)
    {
        selectedItem = 0;
        prevSelection = -1;
        this.availableItems = availableItems;
        this.onItemSelected = onItemSelected;
        this.onBack = onBack;
        gameObject.SetActive(true);
        UpdateItemList();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++selectedItem;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            --selectedItem;
        }
        selectedItem = Mathf.Clamp(selectedItem, 0, availableItems.Count - 1);
        if (selectedItem != prevSelection)
        {
            UpdateUI();
        }
        prevSelection = selectedItem;
        if (Input.GetKeyDown(KeyCode.Z))
        {
            AudioManager.Instance.PlaySE(SFX.CONFIRM);
            onItemSelected?.Invoke(availableItems[selectedItem]);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            AudioManager.Instance.PlaySE(SFX.CANCEL);
            onBack?.Invoke();
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
        UpdateUI();
    }

    private void UpdateUI()
    {
        selectedItem = Mathf.Clamp(selectedItem, 0, availableItems.Count - 1);

        var item = availableItems[selectedItem];
        itemIcon.sprite = item.Icon;
        itemDescription.text = item.Description;

        HandleScrolling();
        UpdateCursor();

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

    public void UpdateCursor()
    {
        inventoryCursor.rectTransform.position = slotUIList[selectedItem].cursorPos.transform.position;
    }

}

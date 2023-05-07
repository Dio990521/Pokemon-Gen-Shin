using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryUIState { ItemSelection, PartySelection, Busy}

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI itemSlotUI;

    [SerializeField] private Image itemIcon;
    [SerializeField] private Text itemDescription;

    [SerializeField] private Image upArrow;
    [SerializeField] private Image downArrow;

    [SerializeField] private PartyScreen partyScreen;
    private InventoryUIState state;

    private Action onItemUsed;

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
        inventory.OnUpdated += UpdateItemList;
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

    public void HandleUpdate(Action onBack, Action onItemUsed=null)
    {
        this.onItemUsed = onItemUsed;
        if (state == InventoryUIState.ItemSelection)
        {
            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.Slots.Count - 1);
            prevSelection = selectedItem;

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ++selectedItem;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                --selectedItem;
            }

            
            if (prevSelection != selectedItem || selectedItem == 0)
            {
                UpdateUI();
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                OpenPartyScreen();
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                onBack?.Invoke();
            }
        } 
        else if (state == InventoryUIState.PartySelection)
        {
            Action onSelected = () =>
            {
                StartCoroutine(UseItem());
            };

            Action onBackPartyScreen = () => 
            { 
                ClosePartyScreen();
            };

            partyScreen.HandleUpdate(onSelected, onBackPartyScreen);
        }


    }

    private IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;
        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember);
        if (usedItem != null)
        {
            yield return DialogueManager.Instance.ShowDialogueText($"你使用了{usedItem.ItemName}！");
            onItemUsed?.Invoke();
        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText($"什么也没有发生！");
        }

        ClosePartyScreen();
        UpdateUI();
    }

    private void UpdateUI()
    {
        
        var item = inventory.Slots[selectedItem].Item;
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

    private void UpdateCursor()
    {
        inventoryCursor.rectTransform.position = slotUIList[selectedItem].cursorPos.transform.position;
    }

    private void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);
    }

    private void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;
        partyScreen.gameObject.SetActive(false);
    }

}

using PokeGenshinUtils.SelectionUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageListUI : SelectionUI<ItemSlotUI>
{
    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI _itemSlotUI;
    [SerializeField] private Image upArrow;
    [SerializeField] private Image downArrow;
    [SerializeField] private Text categoryText;
    [SerializeField] private List<Image> categoryPoints;

    [SerializeField] private Image elementIcon;
    [SerializeField] private List<Sprite> elementIcons;

    private List<ItemSlotUI> slotUIList;
    private Storage _storage;
    private int prevCategory = -1;
    private int selectedCategory = 0;
    private RectTransform itemListRect;

    private const int _itemsInViewPort = 6;

    public int SelectedCategory { get => selectedCategory; set => selectedCategory = value; }

    private void Awake()
    {
        itemListRect = itemList.GetComponent<RectTransform>();
        print(itemListRect);
    }

    public void Init()
    {
        _storage = Storage.GetStorage();
        _storage.OnStorageUpdate += UpdateItemList;
        selectedItem = -1;
        UpdateItemList();
    }

    public bool IsCategoryEmpty()
    {
        return slotUIList.Count == 0;
    }

    public void HideSelection()
    {
        selectedItem = -1;
        prevSelection = -1;
        UpdateSelectionUI();
    }

    private void UpdateItemList()
    {
        // Clear all the existing items
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();

        foreach (var pokemon in _storage.GetSlotsByCategory(SelectedCategory))
        {
            var slotUIObj = Instantiate(_itemSlotUI, itemList.transform);
            slotUIObj.SetPokeData(pokemon);
            slotUIList.Add(slotUIObj);
        }

        SetItems(slotUIList);

        UpdateCategory();
    }

    public override void HandleUpdate(bool allowCancel = true)
    {
        base.HandleUpdate();

        if (prevCategory != SelectedCategory)
        {
            ResetSelection();
            UpdateCategory();
            UpdateItemList();
        }
        prevCategory = SelectedCategory;
    }

    public override void HandleListSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedItem += 1;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedItem -= 1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++SelectedCategory;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --SelectedCategory;
        }
    }

    public override void UpdateSelectionUI()
    {
        base.UpdateSelectionUI();
        HandleScrolling();
    }

    public override void ClampSelection()
    {
        base.ClampSelection();
        if (SelectedCategory > Storage.ElementCategories.Count - 1)
        {
            SelectedCategory = 0;
        }
        else if (SelectedCategory < 0)
        {
            SelectedCategory = Storage.ElementCategories.Count - 1;
        }
    }

    private void HandleScrolling()
    {

        bool showUpArrow = selectedItem > (_itemsInViewPort / 2) && slotUIList.Count > _itemsInViewPort;
        upArrow.gameObject.SetActive(showUpArrow);
        bool showDownArrow = selectedItem + (_itemsInViewPort / 2) < slotUIList.Count && slotUIList.Count > _itemsInViewPort;
        downArrow.gameObject.SetActive(showDownArrow);

        if (selectedItem + (_itemsInViewPort / 2) <= slotUIList.Count)
        {
            float scrollPos = Mathf.Clamp(selectedItem - _itemsInViewPort / 2, 0, selectedItem) * slotUIList[0].Height;
            if (itemListRect!=null)
            {
                itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);
            }
        }
    }

    private void UpdateCategory()
    {
        categoryText.text = Storage.ElementCategories[SelectedCategory];
        foreach (var point in categoryPoints)
        {
            point.color = Color.grey;
        }
        elementIcon.sprite = elementIcons[SelectedCategory];
        categoryPoints[SelectedCategory].color = Color.white;
    }

}

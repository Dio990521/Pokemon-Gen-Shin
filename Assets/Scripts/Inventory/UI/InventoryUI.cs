using DG.Tweening;
using PokeGenshinUtils.SelectionUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : SelectionUI<ItemSlotUI>
{
    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI itemSlotUI;

    [SerializeField] private Image itemIcon;
    [SerializeField] private Text itemDescription;
    [SerializeField] private Text categoryText;

    [SerializeField] private Image upArrow;
    [SerializeField] private Image downArrow;

    [SerializeField] private Image bagIcon;
    [SerializeField] private List<Sprite> bagIcons;

    [SerializeField] private PartyScreen partyScreen;
    [SerializeField] private ForgetMoveSelectionUI moveSelectionUI;

    [SerializeField] private GameObject _leftArrow;
    [SerializeField] private GameObject _rightArrow;

    private const int itemsInViewPort = 10;

    private Inventory inventory;
    private int prevCategory = -1;
    private int selectedCategory = 0;
    private List<ItemSlotUI> slotUIList;
    private RectTransform itemListRect;
    private MoveBase moveToLearn;

    private Vector3 _bagOriginPos;

    public ItemBase SelectedItem => inventory.GetItem(selectedItem, selectedCategory);

    [SerializeField] private List<Image> categoryPoints;

    public int PrevCategory { get => prevCategory; set => prevCategory = value; }
    public int SelectedCategory { get => selectedCategory; set => selectedCategory = value; }

    private Vector3 _leftArrowPos;
    private Vector3 _rightArrowPos;

    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
        _bagOriginPos = bagIcon.transform.localPosition;
        _leftArrowPos = _leftArrow.transform.position;
        _rightArrowPos = _rightArrow.transform.position;
    }

    private void Start()
    {
        UpdateItemList();
        inventory.OnUpdated += UpdateItemList;
    }

    public bool IsCategoryEmpty()
    {
        return slotUIList.Count == 0;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        selectedItem = 0;
        prevSelection = -1;
        prevCategory = -1;
    }

    private void UpdateItemList()
    {
        // Clear all the existing items
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();

        foreach (var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);
            slotUIList.Add(slotUIObj);
        }

        SetItems(slotUIList);

        UpdateUI();
        UpdateCategory();
    }

    public override void HandleUpdate(bool allowCancel = true)
    {
        base.HandleUpdate();

        if (prevCategory != selectedCategory)
        {
            ResetSelection();
            UpdateCategory();
            UpdateItemList();
        }
        prevCategory = selectedCategory;
    }

    public override void HandleListSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            BagShakeAnim();
            selectedItem += 1;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            BagShakeAnim();
            selectedItem -= 1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            AudioManager.Instance.PlaySE(SFX.CONFIRM);
            ++selectedCategory;
            var sequence = DOTween.Sequence();
            _rightArrow.transform.position = _rightArrowPos;
            sequence.Append(_rightArrow.transform.DOMoveX(_rightArrowPos.x + 5f, 0.15f));
            sequence.Append(_rightArrow.transform.DOMoveX(_rightArrowPos.x, 0.15f));
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            AudioManager.Instance.PlaySE(SFX.CONFIRM);
            --selectedCategory;
            var sequence = DOTween.Sequence();
            _leftArrow.transform.position = _leftArrowPos;
            sequence.Append(_leftArrow.transform.DOMoveX(_leftArrowPos.x - 5f, 0.15f));
            sequence.Append(_leftArrow.transform.DOMoveX(_leftArrowPos.x, 0.15f));
        }
    }

    public override void UpdateSelectionUI()
    {
        base.UpdateSelectionUI();

        var slots = inventory.GetSlotsByCategory(selectedCategory);
        if (slots.Count > 0 && selectedItem >= 0)
        {
            selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count - 1);
            var item = slots[selectedItem].Item;
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }
        else
        {
            itemIcon.sprite = null;
            itemDescription.text = null;
        }

        HandleScrolling();
    }

    public override void ClampSelection()
    {
        base.ClampSelection();
        if (selectedCategory > Inventory.ItemCategories.Count - 1)
        {
            selectedCategory = 0;
        }
        else if (selectedCategory < 0)
        {
            selectedCategory = Inventory.ItemCategories.Count - 1;
        }
    }

    private void BagShakeAnim()
    {
        var slots = inventory.GetSlotsByCategory(selectedCategory);
        if (slots.Count > 0)
        {
            AudioManager.Instance.PlaySE(SFX.CURSOR);
            bagIcon.transform.localPosition = _bagOriginPos;
            bagIcon.transform.DOShakePosition(0.5f, 3f);
        }
    }

    private void UpdateUI()
    {
        var slots = inventory.GetSlotsByCategory(selectedCategory);
        selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count-1);

        if (slots.Count > 0)
        {
            var item = slots[selectedItem].Item;
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }
        
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

    private void UpdateCategory()
    {
        categoryText.text = Inventory.ItemCategories[selectedCategory];
        foreach (var point in categoryPoints)
        {
            point.gameObject.SetActive(false);
        }
        bagIcon.sprite = bagIcons[selectedCategory];
        categoryPoints[selectedCategory].gameObject.SetActive(true);
        if (gameObject.activeSelf)
        {
            StartCoroutine(BagAnim());
        }
    }

    private IEnumerator BagAnim()
    {
        bagIcon.transform.localPosition = _bagOriginPos;
        yield return bagIcon.transform.DOPunchPosition(Vector3.up * 3, 0.25f, 1, 3).SetEase(Ease.OutQuad).WaitForCompletion();
    }
}

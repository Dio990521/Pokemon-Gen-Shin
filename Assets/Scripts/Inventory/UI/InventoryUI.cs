using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryUIState { ItemSelection, PartySelection, MoveToForget, Busy}

public class InventoryUI : MonoBehaviour
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
    [SerializeField] private MoveSelectionUI moveSelectionUI;
    private InventoryUIState state;

    private Action<ItemBase> onItemUsed;

    private const int itemsInViewPort = 10;

    private Inventory inventory;
    private int selectedItem;
    private int prevSelection = -1;
    private int prevCategory = -1;
    private int selectedCategory;
    private List<ItemSlotUI> slotUIList;
    private RectTransform itemListRect;
    private MoveBase moveToLearn;

    [SerializeField] private Image inventoryCursor;
    [SerializeField] private List<Image> categoryPoints;

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
        UpdateUI();
        UpdateCategory();
    }

    public void HandleUpdate(Action onBack, Action<ItemBase> onItemUsed=null)
    {
        this.onItemUsed = onItemUsed;
        if (state == InventoryUIState.ItemSelection)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ++selectedItem;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                --selectedItem;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ++selectedCategory;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                --selectedCategory;
            }

            if (selectedCategory > Inventory.ItemCategories.Count - 1)
            {
                selectedCategory = 0;
            }
            else if (selectedCategory < 0)
            {
                selectedCategory = Inventory.ItemCategories.Count - 1;
            }
            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.GetSlotsByCategory(selectedCategory).Count - 1);
            
            if (prevCategory != selectedCategory)
            {
                ResetSelection();
                UpdateCategory();
                UpdateItemList();
            }
            prevCategory = selectedCategory;

            if (prevSelection != selectedItem || selectedItem == 0)
            {
                UpdateUI();
            }
            prevSelection = selectedItem;

            if (Input.GetKeyDown(KeyCode.Z))
            {
                AudioManager.Instance.PlaySE(SFX.CONFIRM);
                StartCoroutine(ItemSelected());
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                AudioManager.Instance.PlaySE(SFX.CANCEL);
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
        else if (state == InventoryUIState.MoveToForget)
        {

            Action<int> onMoveSelected = (int moveIndex) =>
            {
                StartCoroutine(OnMoveToForgetSelected(moveIndex));
            };

            moveSelectionUI.HandleMoveSelection(onMoveSelected);
        }


    }

    public IEnumerator ItemSelected()
    {
        state = InventoryUIState.Busy;

        var item = inventory.GetItem(selectedItem, selectedCategory);
        if (item == null)
        {
            state = InventoryUIState.ItemSelection;
            yield break;
        }

        if (GameManager.Instance.State == GameState.Shop)
        {
            onItemUsed?.Invoke(item);
            state = InventoryUIState.ItemSelection;
            yield break;
        }

        if (GameManager.Instance.State == GameState.Battle)
        {
            // In battle
            if (!item.CanUseInBattle)
            {
                yield return DialogueManager.Instance.ShowDialogueText($"你不能在战斗中使用它！");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        else
        {
            // Outside battle
            if (!item.CanUseOutsideBattle)
            {
                yield return DialogueManager.Instance.ShowDialogueText($"你不能在这里使用它！");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }


        if (selectedCategory == (int)ItemCategory.Pokeballs)
        {
            StartCoroutine(UseItem());
        }
        else
        {
            OpenPartyScreen();
        }
    }

    private IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;

        yield return HandleTmItems();

        var item = inventory.GetItem(selectedItem, selectedCategory);
        var pokemon = partyScreen.SelectedMember;
        if (item is EvolutionItem)
        {
            var evolution = pokemon.CheckForEvolution(item);
            if (evolution != null)
            {
                yield return EvolutionManager.Instance.Evolve(pokemon, evolution);
            }
            else
            {
                yield return DialogueManager.Instance.ShowDialogueText($"什么也没有发生！");
                ClosePartyScreen();
                yield break;
            }
        }


        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory);
        if (usedItem != null)
        {
            if (usedItem is RecoveryItem)
            {
                yield return DialogueManager.Instance.ShowDialogueText($"你使用了{usedItem.ItemName}！");
            }
            onItemUsed?.Invoke(usedItem);
        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText($"什么也没有发生！");
        }

        ClosePartyScreen();
        UpdateUI();
    }

    private IEnumerator HandleTmItems()
    {
        var tmItem = inventory.GetItem(selectedItem, selectedCategory) as TmItem;
        if (tmItem == null)
        {
            yield break;
        }
        var pokemon = partyScreen.SelectedMember;
        if (pokemon.Moves.Count < PokemonBase.MaxNumOfMoves)
        {
            pokemon.LearnMove(tmItem.Move);
            yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.PokemonBase.PokemonName}习得了新技能\n{tmItem.Move.MoveName}！");

        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.PokemonBase.PokemonName}想要学习{tmItem.Move.MoveName}...");
            yield return DialogueManager.Instance.ShowDialogueText($"但是{pokemon.PokemonBase.PokemonName}掌握的技能太多了！");
            yield return ChooseMoveToForget(pokemon, tmItem.Move);
            yield return new WaitUntil(() => state != InventoryUIState.MoveToForget);
        }
    }

    private IEnumerator ChooseMoveToForget(Pokemon pokemon, MoveBase newMove)
    {
        state = InventoryUIState.Busy;
        yield return DialogueManager.Instance.ShowDialogueText($"想要让{pokemon.PokemonBase.PokemonName}\n遗忘哪个技能？", true, false);
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(pokemon.Moves.Select(x => x.MoveBase).ToList(), newMove);
        moveToLearn = newMove;
        state = InventoryUIState.MoveToForget;
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

        if (slots.Count > 0)
        {
            inventoryCursor.gameObject.SetActive(true);
            UpdateCursor();
        }
        else
        {
            inventoryCursor.gameObject.SetActive(false);
        }
            
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

    private void UpdateCategory()
    {
        categoryText.text = Inventory.ItemCategories[selectedCategory];
        foreach (var point in categoryPoints)
        {
            point.gameObject.SetActive(false);
        }
        bagIcon.sprite = bagIcons[selectedCategory];
        categoryPoints[selectedCategory].gameObject.SetActive(true);
    }

    private void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;
        partyScreen.Show();
    }

    private void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;
        partyScreen.gameObject.SetActive(false);
    }

    private void ResetSelection()
    {
        selectedItem = 0;
        prevSelection = -1;
        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);
        itemIcon.sprite = null;
        itemDescription.text = "";
    }

    private IEnumerator OnMoveToForgetSelected(int moveIndex)
    {
        var pokemon = partyScreen.SelectedMember;

        DialogueManager.Instance.CloseDialog();
        moveSelectionUI.gameObject.SetActive(false);
        if (moveIndex == PokemonBase.MaxNumOfMoves)
        {
            // Don't learn the new move
            yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.PokemonBase.PokemonName}放弃学习{moveToLearn.MoveName}！");
        }
        else
        {
            // Forget the selected move and learn new move
            var selevtedMove = pokemon.Moves[moveIndex].MoveBase;
            yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.PokemonBase.PokemonName}忘掉了{selevtedMove.MoveName}！");
            pokemon.Moves[moveIndex] = new Move(moveToLearn);
        }

        moveToLearn = null;
        state = InventoryUIState.ItemSelection;
    }

}

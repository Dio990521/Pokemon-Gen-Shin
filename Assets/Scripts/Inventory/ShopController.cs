using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopState { Menu, Buying, Selling, Busy}

public class ShopController : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private WalletUI walletUI;

    public event Action OnStart;
    public event Action OnFinish;
    private Inventory inventory;

    private ShopState state;

    public static ShopController i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    private void Start()
    {
        inventory = Inventory.GetInventory();
    }

    public IEnumerator StartTrading(Merchant merchant)
    {
        OnStart?.Invoke();
        yield return StartMenuState();
    }

    private IEnumerator StartMenuState()
    {
        state = ShopState.Menu;
        int selectedChoice = 0;
        yield return DialogueManager.Instance.ShowDialogueText("你要干啥？",
            waitForInput: false,
            choices: new List<string>() { "买", "卖", "算了" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            // Buy
            state = ShopState.Buying;
        }
        else if (selectedChoice == 1)
        {
            // Sell
            state = ShopState.Selling;
            inventoryUI.gameObject.SetActive(true);
            
        }
        else if (selectedChoice == 2)
        {
            // Quit
            OnFinish?.Invoke();
            yield break;
        }
    }

    public void HandleUpdate()
    {
        if (state == ShopState.Selling)
        {
            inventoryUI.HandleUpdate(OnBackFromSelling, (selectedItem) => StartCoroutine(SellItem(selectedItem)));
        }
    }

    private void OnBackFromSelling()
    {
        inventoryUI.gameObject.SetActive(false);
        StartCoroutine(StartMenuState());
    }

    private IEnumerator SellItem(ItemBase item)
    {
        state = ShopState.Busy;
        if (!item.IsSellable)
        {
            yield return DialogueManager.Instance.ShowDialogueText("你不能卖掉它！");
            state = ShopState.Selling;
            yield break;
        }

        walletUI.Show();

        int sellingPrice = (int)Mathf.Round(item.Price / 2);
        int selectedChoice = 0;
        yield return DialogueManager.Instance.ShowDialogueText($"我{sellingPrice}收了，你卖不卖？",
            waitForInput: false,
            choices: new List<string>() { "彳亍", "算了" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0) 
        { 
            // Yes
            inventory.RemoveItem(item);
            Wallet.i.AddMoney(sellingPrice);
            yield return DialogueManager.Instance.ShowDialogueText($"成功卖掉{item.ItemName}！\n获得了{sellingPrice}金钱！");

        }
        walletUI.Close();
        state = ShopState.Selling;
    }
}

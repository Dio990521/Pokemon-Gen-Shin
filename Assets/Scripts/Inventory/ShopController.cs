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
        yield return DialogueManager.Instance.ShowDialogueText("��Ҫ��ɶ��",
            waitForInput: false,
            choices: new List<string>() { "��", "��", "����" },
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
            yield return DialogueManager.Instance.ShowDialogueText("�㲻����������");
            state = ShopState.Selling;
            yield break;
        }

        walletUI.Show();

        int sellingPrice = (int)Mathf.Round(item.Price / 2);
        int selectedChoice = 0;
        yield return DialogueManager.Instance.ShowDialogueText($"��{sellingPrice}���ˣ�����������",
            waitForInput: false,
            choices: new List<string>() { "��ء", "����" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0) 
        { 
            // Yes
            inventory.RemoveItem(item);
            Wallet.i.AddMoney(sellingPrice);
            yield return DialogueManager.Instance.ShowDialogueText($"�ɹ�����{item.ItemName}��\n�����{sellingPrice}��Ǯ��");

        }
        walletUI.Close();
        state = ShopState.Selling;
    }
}

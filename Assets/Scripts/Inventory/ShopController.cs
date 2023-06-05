using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopState { Menu, Buying, Selling, Busy}

public class ShopController : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private WalletUI walletUI;
    [SerializeField] private CountSelectorUI countSelectorUI;
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private Vector2 shopCameraOffset;

    public event Action OnStart;
    public event Action OnFinish;
    private Inventory inventory;
    private Merchant merchant;

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
        this.merchant = merchant;
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
            yield return GameManager.Instance.MoveCamera(shopCameraOffset);
            walletUI.Show();
            shopUI.Show(merchant.AvailableItems, (item) => StartCoroutine(BuyItem(item)),
                () => StartCoroutine(OnBackFromBuying()));
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
        else if (state == ShopState.Buying)
        {
            shopUI.HandleUpdate();
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
        int countToSell = 1;
        int itemCount = inventory.GetItemCount(item);
        if (itemCount > 1)
        {
            yield return DialogueManager.Instance.ShowDialogueText("�����ٸ��أ�",
                waitForInput: false, autoClose: false);
            yield return countSelectorUI.ShowSelector(itemCount, sellingPrice,
                (selectedCount) => countToSell = selectedCount);
            DialogueManager.Instance.CloseDialog();
        }

        sellingPrice = sellingPrice * countToSell;

        int selectedChoice = 0;
        yield return DialogueManager.Instance.ShowDialogueText($"��{sellingPrice}���ˣ�����������",
            waitForInput: false,
            choices: new List<string>() { "��ء", "����" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0) 
        { 
            // Yes
            inventory.RemoveItem(item, countToSell);
            Wallet.i.AddMoney(sellingPrice);
            yield return DialogueManager.Instance.ShowDialogueText($"�ɹ�����{item.ItemName}��\n�����{sellingPrice}��Ǯ��");

        }
        walletUI.Close();
        state = ShopState.Selling;
    }

    private IEnumerator BuyItem(ItemBase item)
    {
        state = ShopState.Busy;
        yield return DialogueManager.Instance.ShowDialogueText($"����ٸ���", waitForInput: false, autoClose: false);

        int countToBuy = 1;
        yield return countSelectorUI.ShowSelector(99, item.Price,
            (selectedCount) => countToBuy = selectedCount);

        DialogueManager.Instance.CloseDialog();
        int totalPrice = item.Price * countToBuy;
        if (Wallet.i.HasMoney(totalPrice))
        {
            int selectedChoice = 0;
            yield return DialogueManager.Instance.ShowDialogueText($"һ��{totalPrice}Ԫ������",
            waitForInput: false,
            choices: new List<string>() { "��ء", "����" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

            if (selectedChoice == 0)
            {
                inventory.AddItem(item, countToBuy);
                Wallet.i.TakeMoney(totalPrice);
                yield return DialogueManager.Instance.ShowDialogueText($"��л�ݹˣ��´�������");
            }
        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText($"��û����ô���Ǯ��");
        }
        state = ShopState.Buying;
    }

    private IEnumerator OnBackFromBuying()
    {
        yield return GameManager.Instance.MoveCamera(-shopCameraOffset);
        shopUI.Close();
        walletUI.Close();
        StartCoroutine(StartMenuState());
    }
}

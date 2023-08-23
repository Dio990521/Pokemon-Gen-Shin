using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSellingState : State<GameManager>
{
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private WalletUI walletUI;
    [SerializeField] private CountSelectorUI countSelectorUI;

    public List<ItemBase> AvailableItems { get; set; }

    public static ShopSellingState I { get; private set; }

    private GameManager _gameManager;
    private Inventory inventory;


    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        inventory = Inventory.GetInventory();
    }

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
        StartCoroutine(StartSellingState());
    }

    private IEnumerator StartSellingState()
    {
        yield return _gameManager.StateMachine.PushAndWait(InventoryState.I);

        var selectedItem = InventoryState.I.SelectedItem;
        if (selectedItem != null )
        {
            yield return SellItem(selectedItem);
            StartCoroutine(StartSellingState());
        }
        else
        {
            _gameManager.StateMachine.Pop();
        }
    }

    private IEnumerator SellItem(ItemBase item)
    {
        if (!item.IsSellable)
        {
            yield return DialogueManager.Instance.ShowDialogueText("�㲻����������");
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

        sellingPrice *= countToSell;

        int selectedChoice = 0;
        yield return DialogueManager.Instance.ShowDialogueText($"��{sellingPrice}���ˣ�����������",
            waitForInput: false,
            choices: new List<string>() { "��ء", "����" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex,
            cancelX: false);

        if (selectedChoice == 0)
        {
            // Yes
            inventory.RemoveItem(item, countToSell);
            Wallet.i.AddMoney(sellingPrice);
            yield return DialogueManager.Instance.ShowDialogueText($"�ɹ�����{item.ItemName}��\n�����{sellingPrice}Ħ����");

        }
        walletUI.Close();
    }
}

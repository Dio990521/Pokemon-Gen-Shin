using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopBuyingState : State<GameManager>
{
    [SerializeField] private WalletUI walletUI;
    [SerializeField] private CountSelectorUI countSelectorUI;
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private Vector2 shopCameraOffset;

    public List<ItemBase> AvailableItems { get; set; }
    public static ShopBuyingState I { get; private set; }

    private GameManager _gameManager;

    private Inventory inventory;
    private bool _browseItems = false;

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
        _browseItems = false;
        shopUI.OnSelected += OnItemSelected;
        shopUI.OnBack += OnBack;
        StartCoroutine(StartBuyingState());
    }

    public override void Execute()
    {
        if (_browseItems)
        {
            shopUI.HandleUpdate();
        }
    }

    public override void Exit(bool sfx = true)
    {
        shopUI.OnSelected -= OnItemSelected;
        shopUI.OnBack -= OnBack;
        shopUI.ResetSelection();
    }

    private void OnItemSelected(int selection)
    {
        StartCoroutine(BuyItem(AvailableItems[selection]));
    }

    private void OnBack()
    {
        StartCoroutine(OnBackFromBuying());
    }

    private IEnumerator StartBuyingState()
    {
        yield return GameManager.Instance.MoveCamera(shopCameraOffset);
        walletUI.Show();
        shopUI.Show(AvailableItems);
        _browseItems = true;
    }

    private IEnumerator BuyItem(ItemBase item)
    {

        _browseItems = false;

        yield return DialogueManager.Instance.ShowDialogueText($"����ٸ���", waitForInput: false, autoClose: false);

        int countToBuy = 1;
        yield return countSelectorUI.ShowSelector(99, item.Price,
            (selectedCount) => countToBuy = selectedCount);

        DialogueManager.Instance.CloseDialog();
        int totalPrice = item.Price * countToBuy;
        if (Wallet.i.HasMoney(totalPrice))
        {
            yield return DialogueManager.Instance.ShowDialogueText($"һ��{totalPrice}Ħ��������", autoClose: false);
            ChoiceState.I.Choices = new List<string>() { "��ء", "����" };
            yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

            int selectedChoice = ChoiceState.I.Selection;

            if (selectedChoice == 0)
            {
                inventory.AddItem(item, countToBuy);
                Wallet.i.TakeMoney(totalPrice);
                yield return DialogueManager.Instance.ShowDialogueText($"��л�ݹˣ��´�������");
            }
        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText($"��û����ô��Ħ����");
        }

        _browseItems = true;
    }

    private IEnumerator OnBackFromBuying()
    {
        yield return GameManager.Instance.MoveCamera(-shopCameraOffset);
        shopUI.Close();
        walletUI.Close();
        _gameManager.StateMachine.Pop();
    }


}

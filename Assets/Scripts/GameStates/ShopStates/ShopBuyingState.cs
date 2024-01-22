using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopBuyingState : State<GameManager>
{
    [SerializeField] private WalletUI walletUI;
    [SerializeField] private CountSelectorUI countSelectorUI;
    [SerializeField] private ShopUI shopUI;

    public List<ItemBase> AvailableItems { get; set; }
    public List<PokemonBase> BuyablePokemons { get; set; }


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
        var prevState = _gameManager.StateMachine.GetPrevState();
        if (prevState == ShopMenuState.I)
        {
            StartCoroutine(BuyItem(AvailableItems[selection]));
        }
        else if (prevState == PCMenuState.I)
        {
            StartCoroutine(BuyYuanshiItem(AvailableItems[selection]));
        }

    }

    private void OnBack()
    {
        StartCoroutine(OnBackFromBuying());
    }

    private IEnumerator StartBuyingState()
    {
        yield return GameManager.Instance.MoveCamera(ShopMenuState.I.CameraOffset);
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
        if (Wallet.I.HasMoney(totalPrice))
        {
            yield return DialogueManager.Instance.ShowDialogueText($"һ��{totalPrice}Ħ��������", autoClose: false);
            ChoiceState.I.Choices = new List<string>() { "��ء", "����" };
            yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

            int selectedChoice = ChoiceState.I.Selection;

            if (selectedChoice == 0)
            {
                AudioManager.Instance.PlaySE(SFX.BUY);
                inventory.AddItem(item, countToBuy, false);
                Wallet.I.TakeMoney(totalPrice);
                yield return DialogueManager.Instance.ShowDialogueText($"��л�ݹˣ��´�������");
            }
        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText($"��û����ô��Ħ����");
        }

        _browseItems = true;
    }

    private IEnumerator BuyYuanshiItem(ItemBase item)
    {
        _browseItems = false;
        yield return DialogueManager.Instance.ShowDialogueText($"����ٸ���", waitForInput: false, autoClose: false);

        int countToBuy = 1;
        yield return countSelectorUI.ShowSelector(99, item.YuanshiPrice,
            (selectedCount) => countToBuy = selectedCount, true);

        DialogueManager.Instance.CloseDialog();
        int totalPrice = item.YuanshiPrice * countToBuy;
        if (Inventory.GetInventory().GetItemCount(Wallet.I.Yuanshi) >= totalPrice)
        {
            yield return DialogueManager.Instance.ShowDialogueText($"һ����Ҫ����{totalPrice}ԭʯ������", autoClose: false);
            ChoiceState.I.Choices = new List<string>() { "��ء", "����" };
            yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

            int selectedChoice = ChoiceState.I.Selection;

            if (selectedChoice == 0)
            {
                AudioManager.Instance.PlaySE(SFX.BUY);
                inventory.AddItem(item, countToBuy, false);
                Inventory.GetInventory().RemoveItem(Wallet.I.Yuanshi, totalPrice);
                Wallet.I.TakeMoney(0);
                yield return DialogueManager.Instance.ShowDialogueText($"��л�ݹˣ��´�������");
            }
        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText($"��û����ô��ԭʯ��");
        }
        _browseItems = true;
    }

    private IEnumerator OnBackFromBuying()
    {
        yield return GameManager.Instance.MoveCamera(-ShopMenuState.I.CameraOffset);
        shopUI.Close();
        walletUI.Close();
        _gameManager.StateMachine.Pop();
    }


}

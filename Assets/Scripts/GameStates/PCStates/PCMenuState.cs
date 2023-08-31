using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCMenuState : State<GameManager>
{
    [SerializeField] private List<ItemBase> availableItems;
    [SerializeField] private WalletUI walletUI;
    [SerializeField] private ShopUI yuanshiShopUI;
    [SerializeField] private CountSelectorUI countSelectorUI;
    [SerializeField] private GameObject pokemonRoom;

    private Inventory _inventory;

    public static PCMenuState I { get; private set; }

    private GameManager _gameManager;

    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        _inventory = Inventory.GetInventory();
    }

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
        StartCoroutine(Boost());
    }

    public override void Execute()
    {

    }

    public override void Exit(bool sfx = true)
    {

    }

    public IEnumerator Boost()
    {
        AudioManager.Instance.PlaySE(SFX.PC_ON);
        //_computer = computer;
        yield return StartMenuState();
    }

    private IEnumerator StartMenuState()
    {
 
        yield return DialogueManager.Instance.ShowDialogueText("ʹ�õ�����Щʲô�أ�", autoClose: false);
        ChoiceState.I.Choices = new List<string>() { "ԭʯ��ֵ", "ԭʯ�̵�", "�����βֿ�", "�һؼ���" };
        yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

        int selectedChoice = ChoiceState.I.Selection;

        if (selectedChoice == 0)
        {
            AudioManager.Instance.PlaySE(SFX.PC_OPERATE);
            walletUI.Show();
            yield return BuyYuanshi();
            yield return StartMenuState();
        }
        else if (selectedChoice == 1)
        {
            AudioManager.Instance.PlaySE(SFX.PC_OPERATE);
            ShopBuyingState.I.AvailableItems = availableItems;
            yield return GameManager.Instance.StateMachine.PushAndWait(ShopBuyingState.I);
            yield return StartMenuState();
        }
        else if (selectedChoice == 2)
        {
            AudioManager.Instance.PlaySE(SFX.PC_OPERATE);
            pokemonRoom.gameObject.SetActive(true);
        }
        else if (selectedChoice == 3)
        {

        }
        else if (selectedChoice == -1)
        {
            AudioManager.Instance.PlaySE(SFX.PC_OFF);
            _gameManager.StateMachine.Pop();
        }
    }

    private IEnumerator BuyYuanshi()
    {
        yield return DialogueManager.Instance.ShowDialogueText("С����飬�����������ע����ơ�\n��ע�⣺�����񲻻���׹��ε�Ӫ���������Ӱ�졣");
        yield return DialogueManager.Instance.ShowDialogueText("��Ҫ��ֵ����ԭʯ�أ�", autoClose: false);
        ChoiceState.I.Choices = new List<string>() { "60", "300", "980", "1980", "3280", "6480" };
        yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

        int selectedChoice = ChoiceState.I.Selection;

        switch (selectedChoice)
        {
            case -1:
                walletUI.Close();
                break;
            case 0:
                yield return BuyYuanshiConfirm(60);
                break;
            case 1:
                yield return BuyYuanshiConfirm(300);
                break;
            case 2:
                yield return BuyYuanshiConfirm(980);
                break;
            case 3:
                yield return BuyYuanshiConfirm(1980);
                break;
            case 4:
                yield return BuyYuanshiConfirm(3280);
                break;
            case 5:
                yield return BuyYuanshiConfirm(6480);
                break;
        }
    }

    private IEnumerator BuyYuanshiConfirm(int yuanshiAmount)
    {
        DialogueManager.Instance.CloseDialog();
        int totalPrice = yuanshiAmount * 10;
        yield return DialogueManager.Instance.ShowDialogueText($"��ֵ{yuanshiAmount}ԭʯ��Ҫ����{totalPrice}Ħ����\nȷ�ϳ�ֵ��", autoClose: false);
        ChoiceState.I.Choices = new List<string>() { "�ݺݵس�", "�����Լ�" };
        yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

        int selectedChoice = ChoiceState.I.Selection;

        if (selectedChoice == 0)
        {
            if (Wallet.i.HasMoney(totalPrice))
            {
                _inventory.AddItem(Wallet.i.Yuanshi, yuanshiAmount);
                Wallet.i.TakeMoney(totalPrice);
                yield return DialogueManager.Instance.ShowDialogueText($"��л�ݹˣ�봽�Ŀ��ֹ��ס��");
            }
            else
            {
                yield return DialogueManager.Instance.ShowDialogueText($"��û����ô��Ħ����");
            }

        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText($"���ĺã���Ҫ���������");
        }
        walletUI.Close();
    }

}

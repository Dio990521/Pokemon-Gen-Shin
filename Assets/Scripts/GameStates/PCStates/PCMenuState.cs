using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PCMenuState : State<GameManager>
{
    [SerializeField] private List<ItemBase> availableItems;
    [SerializeField] private WalletUI walletUI;
    [SerializeField] private ShopUI yuanshiShopUI;
    [SerializeField] private CountSelectorUI countSelectorUI;
    [SerializeField] private GameObject pokemonRoom;

    [SerializeField] private ItemBase _creditCard;

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
            if (!Inventory.GetInventory().HasItem(_creditCard))
            {
                yield return DialogueManager.Instance.ShowDialogueText("��ֵ������Ҫ���ÿ���");
                yield return StartMenuState();
                yield break;
            }
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
            yield return GameManager.Instance.StateMachine.PushAndWait(StorageState.I);
            yield return StartMenuState();
        }
        else if (selectedChoice == 3)
        {
            AudioManager.Instance.PlaySE(SFX.PC_OPERATE);
            GameManager.Instance.PartyScreen.SetMessageText("ѡ����Ҫ�һؼ��ܵı����Ρ�");
            yield return GameManager.Instance.StateMachine.PushAndWait(PartyState.I);
            var selectedPokemon = PartyState.I.SelectedPokemon;
            if (selectedPokemon != null)
            {
                var currentLearnableMoves = selectedPokemon.GetLearnableMovesAtCurrentLevel();
                if (currentLearnableMoves.Count > 0)
                {
                    yield return DialogueManager.Instance.ShowDialogueText("ѡ����Ҫ�һصļ��ܡ�", autoClose: false);
                    ChoiceState.I.Choices = currentLearnableMoves.Select(move => move.MoveBase.MoveName).ToList();
                    yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

                    int selection = ChoiceState.I.Selection;
                    if (selection == -1)
                    {
                        _gameManager.StateMachine.Pop();
                        yield break;
                    }
                    var selectedLearnableMove = currentLearnableMoves[selection];
                    if (selectedPokemon.Moves.Select(move => move.MoveBase).ToList().Contains(selectedLearnableMove.MoveBase))
                    {
                        yield return DialogueManager.Instance.ShowDialogueText("��������Ѿ�ѧ���ˣ�", autoClose: false);
                        yield return StartMenuState();
                        yield break;
                    }
                    yield return DialogueManager.Instance.ShowDialogueText($"��Ҫ��{selectedPokemon.PokemonBase.PokemonName}\n�����ĸ����ܣ�", autoClose: false);
                    MoveToForgetState.I.NewMove = selectedLearnableMove.MoveBase;
                    MoveToForgetState.I.CurrentMoves = selectedPokemon.Moves.Select(m => m.MoveBase).ToList();
                    yield return GameManager.Instance.StateMachine.PushAndWait(MoveToForgetState.I);

                    int moveIndex = MoveToForgetState.I.Selection;
                    if (moveIndex == PokemonBase.MaxNumOfMoves || moveIndex == -1)
                    {
                        // Don't learn the new move
                        yield return DialogueManager.Instance.ShowDialogueText($"{selectedPokemon.PokemonBase.PokemonName}����ѧϰ{selectedLearnableMove.MoveBase.MoveName}��", autoClose: false);
                    }
                    else
                    {
                        // Forget the selected move and learn new move
                        var selevtedMove = selectedPokemon.Moves[moveIndex].MoveBase;
                        yield return DialogueManager.Instance.ShowDialogueText($"{selectedPokemon.PokemonBase.PokemonName}������{selevtedMove.MoveName}��", autoClose: false);
                        selectedPokemon.Moves[moveIndex] = new Move(selectedLearnableMove.MoveBase);
                    }
                    yield return StartMenuState();
                }
                else
                {
                    yield return DialogueManager.Instance.ShowDialogueText("û�п����һصļ��ܣ�", autoClose: false);
                    yield return StartMenuState();
                }
            }
            else
            {
                yield return StartMenuState();
            }
            

        }
        else if (selectedChoice == -1)
        {
            AudioManager.Instance.PlaySE(SFX.PC_OFF);
            _gameManager.StateMachine.Pop();
        }
    }

    private IEnumerator BuyYuanshi()
    {
        yield return DialogueManager.Instance.ShowDialogueText($"С����飬�����������ע����ơ�\n�������ÿ����ö�Ȼ�ʣ��{Wallet.I.VisaLimit}Ħ��");
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
        if (totalPrice > Wallet.I.VisaLimit)
        {
            yield return DialogueManager.Instance.ShowDialogueText($"��ֵ{yuanshiAmount}ԭʯ��Ҫ����{totalPrice}Ħ����\n�����������ÿ����ö�Ȳ��㣡", autoClose: false);
            walletUI.Close();
            yield break;
        }
        yield return DialogueManager.Instance.ShowDialogueText($"��ֵ{yuanshiAmount}ԭʯ��Ҫ����{totalPrice}Ħ����\nȷ�ϳ�ֵ��", autoClose: false);
        ChoiceState.I.Choices = new List<string>() { "�ݺݵس�", "�����Լ�" };
        yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

        int selectedChoice = ChoiceState.I.Selection;

        if (selectedChoice == 0)
        {
            if (Wallet.I.HasMoney(totalPrice))
            {
                _inventory.AddItem(Wallet.I.Yuanshi, yuanshiAmount);
                Wallet.I.TakeMoney(totalPrice);
                Wallet.I.DecreaseVisaLimit(totalPrice);
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

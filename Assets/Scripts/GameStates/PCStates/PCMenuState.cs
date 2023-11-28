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
 
        yield return DialogueManager.Instance.ShowDialogueText("使用电脑做些什么呢？", autoClose: false);
        ChoiceState.I.Choices = new List<string>() { "原石充值", "原石商店", "宝可梦仓库", "找回技能" };
        yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

        int selectedChoice = ChoiceState.I.Selection;

        if (selectedChoice == 0)
        {
            if (!Inventory.GetInventory().HasItem(_creditCard))
            {
                yield return DialogueManager.Instance.ShowDialogueText("充值服务需要信用卡！");
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
            GameManager.Instance.PartyScreen.SetMessageText("选择想要找回技能的宝可梦。");
            yield return GameManager.Instance.StateMachine.PushAndWait(PartyState.I);
            var selectedPokemon = PartyState.I.SelectedPokemon;
            if (selectedPokemon != null)
            {
                var currentLearnableMoves = selectedPokemon.GetLearnableMovesAtCurrentLevel();
                if (currentLearnableMoves.Count > 0)
                {
                    yield return DialogueManager.Instance.ShowDialogueText("选择想要找回的技能。", autoClose: false);
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
                        yield return DialogueManager.Instance.ShowDialogueText("这个技能已经学会了！", autoClose: false);
                        yield return StartMenuState();
                        yield break;
                    }
                    yield return DialogueManager.Instance.ShowDialogueText($"想要让{selectedPokemon.PokemonBase.PokemonName}\n遗忘哪个技能？", autoClose: false);
                    MoveToForgetState.I.NewMove = selectedLearnableMove.MoveBase;
                    MoveToForgetState.I.CurrentMoves = selectedPokemon.Moves.Select(m => m.MoveBase).ToList();
                    yield return GameManager.Instance.StateMachine.PushAndWait(MoveToForgetState.I);

                    int moveIndex = MoveToForgetState.I.Selection;
                    if (moveIndex == PokemonBase.MaxNumOfMoves || moveIndex == -1)
                    {
                        // Don't learn the new move
                        yield return DialogueManager.Instance.ShowDialogueText($"{selectedPokemon.PokemonBase.PokemonName}放弃学习{selectedLearnableMove.MoveBase.MoveName}！", autoClose: false);
                    }
                    else
                    {
                        // Forget the selected move and learn new move
                        var selevtedMove = selectedPokemon.Moves[moveIndex].MoveBase;
                        yield return DialogueManager.Instance.ShowDialogueText($"{selectedPokemon.PokemonBase.PokemonName}忘掉了{selevtedMove.MoveName}！", autoClose: false);
                        selectedPokemon.Moves[moveIndex] = new Move(selectedLearnableMove.MoveBase);
                    }
                    yield return StartMenuState();
                }
                else
                {
                    yield return DialogueManager.Instance.ShowDialogueText("没有可以找回的技能！", autoClose: false);
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
        yield return DialogueManager.Instance.ShowDialogueText($"小氪怡情，大氪伤身，请您注意节制。\n您的信用卡可用额度还剩：{Wallet.I.VisaLimit}摩拉");
        yield return DialogueManager.Instance.ShowDialogueText("您要充值多少原石呢？", autoClose: false);
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
            yield return DialogueManager.Instance.ShowDialogueText($"充值{yuanshiAmount}原石需要消费{totalPrice}摩拉。\n但是您的信用卡可用额度不足！", autoClose: false);
            walletUI.Close();
            yield break;
        }
        yield return DialogueManager.Instance.ShowDialogueText($"充值{yuanshiAmount}原石需要消费{totalPrice}摩拉。\n确认充值吗？", autoClose: false);
        ChoiceState.I.Choices = new List<string>() { "狠狠地充", "克制自己" };
        yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

        int selectedChoice = ChoiceState.I.Selection;

        if (selectedChoice == 0)
        {
            if (Wallet.I.HasMoney(totalPrice))
            {
                _inventory.AddItem(Wallet.I.Yuanshi, yuanshiAmount);
                Wallet.I.TakeMoney(totalPrice);
                Wallet.I.DecreaseVisaLimit(totalPrice);
                yield return DialogueManager.Instance.ShowDialogueText($"多谢惠顾，氪金的快感止不住！");
            }
            else
            {
                yield return DialogueManager.Instance.ShowDialogueText($"你没有那么多摩拉！");
            }

        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText($"做的好！不要输给欲望！");
        }
        walletUI.Close();
    }

}

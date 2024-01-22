using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryState : State<GameManager>
{
    [SerializeField] private InventoryUI _inventoryUI;
    private Inventory _inventory;

    public ItemBase SelectedItem { get; private set; }

    public static InventoryState I { get; private set; }

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
        //AudioManager.Instance.PlaySE(SFX.CONFIRM);
        _gameManager = owner;
        SelectedItem = null;
        _inventoryUI.gameObject.SetActive(true);
        _inventoryUI.OnSelected += OnItemSelected;
        _inventoryUI.OnBack += OnBack;
    }

    public override void Execute()
    {
        _inventoryUI.HandleUpdate();
    }

    public override void Exit(bool sfx)
    {
        _inventoryUI.SelectedCategory = 0;
        _inventoryUI.PrevCategory = -1;
        _inventoryUI.gameObject.SetActive(false);
        _inventoryUI.OnSelected -= OnItemSelected;
        _inventoryUI.OnBack -= OnBack;
    }

    private void OnItemSelected(int selection)
    {
        SelectedItem = _inventoryUI.SelectedItem;

        if (_gameManager.StateMachine.GetPrevState() != ShopSellingState.I)
        {
            StartCoroutine(SelectPokemonAndUseItem());
        }
        else
        {
            _gameManager.StateMachine.Pop();
        }
    }

    private void OnBack()
    {
        SelectedItem = null;
        _gameManager.StateMachine.Pop();
    }

    private IEnumerator SelectPokemonAndUseItem()
    {
        if (!_inventoryUI.IsCategoryEmpty())
        {
            var prevState = _gameManager.StateMachine.GetPrevState();
            if (prevState == BattleState.I) 
            {
                // In Battle
                if (!SelectedItem.CanUseInBattle)
                {
                    yield return DialogueManager.Instance.ShowDialogueText("你不能在这里使用它！");
                    yield break;
                }
            }
            else
            {
                // Outside Battle
                if (!SelectedItem.CanUseOutsideBattle)
                {
                    yield return DialogueManager.Instance.ShowDialogueText("你不能在这里使用它！");
                    yield break;
                }
            }

            if (SelectedItem is PokeballItem)
            {
                _gameManager.StateMachine.Pop();
                yield break;
            }
            else if (SelectedItem is AvoidPokemonItem)
            {
                var playerController = GameManager.Instance.PlayerController;
                if (playerController.AvoidWildPokemon)
                {
                    yield return DialogueManager.Instance.ShowDialogueText("你已经很臭了！呕！");
                }
                else
                {
                    GameManager.Instance.PlayerController.AvoidWildPokemon = true;
                    _inventory.UseItem(SelectedItem, null);
                    yield return DialogueManager.Instance.ShowDialogueText("臭气熏天！宝可梦被熏到不敢靠近！");
                }
                yield break;
            }
            else if (SelectedItem is PoketTransport)
            {
                var playerController = GameManager.Instance.PlayerController;
                List<string> teleports = TeleportManager.Instance.GetActiveList();
                List<int> indices = TeleportManager.Instance.GetActiveTeleportIndex();
                if (teleports.Count < 1)
                {
                    yield return DialogueManager.Instance.ShowDialogueText("没有可以传送的地点！");
                    yield break;
                }
                yield return DialogueManager.Instance.ShowDialogueText($"要传送到哪里呢？", autoClose: false);
                ChoiceState.I.Choices = TeleportManager.Instance.GetActiveList();
                yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

                int selectedChoice = ChoiceState.I.Selection;

                if (selectedChoice != -1)
                {
                    _gameManager.StateMachine.Pop();
                    _gameManager.StateMachine.Pop();
                    yield return Teleport.StartTeleport(TeleportManager.Instance.Teleports[indices[selectedChoice]].SpawnPoint, playerController);
                }
                yield break;
            }
            else if (SelectedItem is EscapeItem)
            {
                yield return DialogueManager.Instance.ShowDialogueText($"确定要抛弃10%的原石\n强制逃离这场战斗吗？", waitForInput: false, autoClose: false);
                ChoiceState.I.Choices = new List<string>() { "润！", "算了" };
                yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

                int selectedChoice = ChoiceState.I.Selection;

                if (selectedChoice == 0)
                {
                    if (prevState == BattleState.I)
                    {
                        if (Wallet.I.TryTakeYuanshiPercentage(0.1f))
                        {
                            _gameManager.StateMachine.Pop();
                            var battleSystem = BattleState.I.BattleSystem;
                            battleSystem.IsBattleOver = true;
                            battleSystem.DialogueBox.TypeDialogue("强行逃跑了！");
                            AudioManager.Instance.PlaySE(SFX.ESCAPE);
                            yield return Fader.FadeIn(1f);
                            yield return TeleportManager.Instance.GameOverTransport(false);
                            yield return battleSystem.BattleOver(false);
                        }
                        else
                        {
                            yield return DialogueManager.Instance.ShowDialogueText($"掏不出原石！\n看来只能继续战斗了！", autoClose: true);
                        }

                    }
                        
                }
                yield break;
            }

            if (SelectedItem is PaimengItem)
            {
                if (prevState == BattleState.I)
                {
                    var battleSystem = BattleState.I.BattleSystem;
                    var enemy = BattleState.I.BattleSystem.EnemyUnit.pokemon.PokemonBase;
                    var effectivenessData = enemy.EffectivenessData;
                    string weakPoints = "";
                    string strongPoints = "";
                    foreach (var effectiveness in effectivenessData)
                    {
                        var elementReactionName = ElementReactionUtil.GetPassiveString(effectiveness.Key);
                        if (effectiveness.Value > 1f)
                        {
                            weakPoints += elementReactionName + "、";
                        }
                        else if (effectiveness.Value < 1f)
                        {
                            strongPoints += elementReactionName + "、";
                        }
                    }
                    _inventoryUI.AllowUpdate = false;
                    _inventoryUI.gameObject.SetActive(false);
                    ActionSelectionState.I.SelectionUI.gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE(SFX.USE_PAIMENG);
                    yield return battleSystem.DialogueBox.TypeDialogue("派蒙瞪大了双眼！");
                    if (weakPoints.Length == 0 && strongPoints.Length == 0)
                    {
                        yield return battleSystem.DialogueBox.TypeDialogue($"{enemy.PokemonName}好像没什么特别的！");
                    }

                    if (weakPoints.Length > 0)
                    {
                        AudioManager.Instance.PlaySE(SFX.FIND_WEAKPOINT);
                        weakPoints = weakPoints.Substring(0, weakPoints.Length - 1);
                        yield return battleSystem.DialogueBox.TypeDialogue($"{weakPoints}可对{enemy.PokemonName}造成更高伤害！");
                    }
                    if (strongPoints.Length > 0)
                    {
                        AudioManager.Instance.PlaySE(SFX.FIND_STRONGPOINT);
                        strongPoints = strongPoints.Substring(0, strongPoints.Length - 1);
                        if (strongPoints.Length > 15)
                        {
                            yield return battleSystem.DialogueBox.TypeDialogue($"{enemy.PokemonName}竟然对所有元素反应的抗性都很高！");
                        }
                        else
                        {
                            yield return battleSystem.DialogueBox.TypeDialogue($"{strongPoints}对{enemy.PokemonName}的伤害较低！");
                        }
                    }
                    RunTurnState.I.SkipEnemyTurn = true;
                    _gameManager.StateMachine.Pop();
                    yield break;
                }
                else
                {
                    yield return _gameManager.StateMachine.PushAndWait(PartyState.I);
                }
            }
            else
            {
                yield return _gameManager.StateMachine.PushAndWait(PartyState.I);
            }

            if (prevState == BattleState.I)
            {
                if (UseItemState.I.ItemUsed)
                {
                    _gameManager.StateMachine.Pop();
                }
            }
        }

    }
}

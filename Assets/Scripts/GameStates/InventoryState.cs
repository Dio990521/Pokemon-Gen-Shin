using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

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
        //AudioManager.Instance.PlaySE(SFX.CANCEL);
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
                    yield return DialogueManager.Instance.ShowDialogueText("�㲻��������ʹ������");
                    yield break;
                }
            }
            else
            {
                // Outside Battle
                if (!SelectedItem.CanUseOutsideBattle)
                {
                    yield return DialogueManager.Instance.ShowDialogueText("�㲻��������ʹ������");
                    yield break;
                }
            }

            if (SelectedItem is PokeballItem)
            {
                _inventory.UseItem(SelectedItem, null);
                _gameManager.StateMachine.Pop();
                yield break;
            }
            else if (SelectedItem is AvoidPokemonItem)
            {
                var playerController = GameManager.Instance.PlayerController;
                if (playerController.AvoidWildPokemon)
                {
                    yield return DialogueManager.Instance.ShowDialogueText("���Ѿ��ܳ��ˣ�Ż��");
                }
                else
                {
                    GameManager.Instance.PlayerController.AvoidWildPokemon = true;
                    _inventory.UseItem(SelectedItem, null);
                    yield return DialogueManager.Instance.ShowDialogueText("����Ѭ�죡�����α�Ѭ�����ҿ�����");
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
                    yield return DialogueManager.Instance.ShowDialogueText("û�п��Դ��͵ĵص㣡");
                    yield break;
                }
                yield return DialogueManager.Instance.ShowDialogueText($"Ҫ���͵������أ�", autoClose: false);
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
                            weakPoints += elementReactionName + "��";
                        }
                        else if (effectiveness.Value < 1f)
                        {
                            strongPoints += elementReactionName + "��";
                        }
                    }
                    _inventoryUI.AllowUpdate = false;
                    _inventoryUI.gameObject.SetActive(false);
                    ActionSelectionState.I.SelectionUI.gameObject.SetActive(false);
                    AudioManager.Instance.PlaySE(SFX.USE_PAIMENG);
                    yield return battleSystem.DialogueBox.TypeDialogue("���ɵɴ���˫�ۣ�");
                    if (weakPoints.Length == 0 && strongPoints.Length == 0)
                    {
                        yield return battleSystem.DialogueBox.TypeDialogue($"{enemy.PokemonName}����ûʲô�ر�ģ�");
                    }

                    if (weakPoints.Length > 0)
                    {
                        AudioManager.Instance.PlaySE(SFX.FIND_WEAKPOINT);
                        weakPoints = weakPoints.Substring(0, weakPoints.Length - 1);
                        yield return battleSystem.DialogueBox.TypeDialogue($"{enemy.PokemonName}��{weakPoints}�Ŀ��Խϲ");
                    }
                    if (strongPoints.Length > 0)
                    {
                        AudioManager.Instance.PlaySE(SFX.FIND_STRONGPOINT);
                        strongPoints = strongPoints.Substring(0, strongPoints.Length - 1);
                        yield return battleSystem.DialogueBox.TypeDialogue($"{enemy.PokemonName}��{strongPoints}�Ŀ��Խϸߣ�");
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

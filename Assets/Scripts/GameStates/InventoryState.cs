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
                _inventory.UseItem(SelectedItem, null);
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


            yield return _gameManager.StateMachine.PushAndWait(PartyState.I);

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

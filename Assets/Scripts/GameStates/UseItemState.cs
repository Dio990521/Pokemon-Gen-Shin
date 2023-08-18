using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UseItemState : State<GameManager>
{
    [SerializeField] private PartyScreen _partyScreen;
    [SerializeField] private InventoryUI _inventoryUI;
    private Inventory _inventory;

    public static UseItemState I { get; private set; }

    private GameManager _gameManager;

    private void Awake()
    {
        I = this;
        _inventory = Inventory.GetInventory();
    }

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
        StartCoroutine(UseItem());
    }

    private IEnumerator UseItem()
    {

        yield return HandleTmItems();

        var item = _inventoryUI.SelectedItem;
        var pokemon = _partyScreen.SelectedMember;

        if (item is TmItem)
        {
            yield return HandleTmItems();
        }
        else
        {
            if (item is EvolutionItem)
            {
                var evolution = pokemon.CheckForEvolution(item);
                if (evolution != null)
                {
                    yield return EvolutionManager.Instance.Evolve(pokemon, evolution);
                }
                else
                {
                    yield return DialogueManager.Instance.ShowDialogueText($"什么也没有发生！");
                    _gameManager.StateMachine.Pop();
                    yield break;
                }
            }


            var usedItem = _inventory.UseItem(item, _partyScreen.SelectedMember);
            if (usedItem != null)
            {
                if (usedItem is RecoveryItem)
                {
                    yield return DialogueManager.Instance.ShowDialogueText($"你使用了{usedItem.ItemName}！");
                }
            }
            else
            {
                if (_inventoryUI.SelectedCategory == (int)ItemCategory.Items)
                {
                    yield return DialogueManager.Instance.ShowDialogueText($"什么也没有发生！");
                }
            }
        }

        _gameManager.StateMachine.Pop();

    }

    private IEnumerator HandleTmItems()
    {
        var tmItem = _inventoryUI.SelectedItem as TmItem;
        if (tmItem == null)
        {
            yield break;
        }
        var pokemon = _partyScreen.SelectedMember;
        if (pokemon.Moves.Count < PokemonBase.MaxNumOfMoves)
        {
            pokemon.LearnMove(tmItem.Move);
            yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.PokemonBase.PokemonName}习得了新技能\n{tmItem.Move.MoveName}！");

        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.PokemonBase.PokemonName}想要学习{tmItem.Move.MoveName}...");
            yield return DialogueManager.Instance.ShowDialogueText($"但是{pokemon.PokemonBase.PokemonName}掌握的技能太多了！");
            //yield return ChooseMoveToForget(pokemon, tmItem.Move);
            //yield return new WaitUntil(() => state != InventoryUIState.MoveToForget);
        }
    }

}

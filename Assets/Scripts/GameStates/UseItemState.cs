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

    public bool ItemUsed { get; private set; }

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
        ItemUsed = false;
        StartCoroutine(UseItem());
    }

    private IEnumerator UseItem()
    {

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
                    yield return EvolutionState.I.Evolve(pokemon, evolution);
                }
                else
                {
                    yield return DialogueManager.Instance.ShowDialogueText("ʲôҲû�з�����");
                    _gameManager.StateMachine.Pop();
                    yield break;
                }
            }
            else if (item is BoostItem)
            {
                var boostItem = item as BoostItem;
                if (boostItem.IsExBoost)
                {
                    pokemon.AddExStatusBias(boostItem.BoostValue);
                    yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.PokemonBase.PokemonName}ͻ�������ң�");
                }
                else
                {
                    if (pokemon.TryAddBias(boostItem.BoostValue))
                    {
                        yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.PokemonBase.PokemonName}�����ǿ�ˣ�");
                        _inventory.UseItem(item, _partyScreen.SelectedMember);
                    }
                    else
                    {
                        yield return DialogueManager.Instance.ShowDialogueText("ʲôҲû�з�����");
                    }
                    _gameManager.StateMachine.Pop();
                    yield break;
                }
            }

            var usedItem = _inventory.UseItem(item, _partyScreen.SelectedMember);
            if (usedItem != null)
            {
                ItemUsed = true;
                yield return DialogueManager.Instance.ShowDialogueText($"��ʹ����{usedItem.ItemName}��");

            }
            else
            {
                if (item is PaimengItem)
                {
                    yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.PokemonBase.PokemonName}ҧ��һ�����ɣ�\n���㣡");
                }
                else
                {
                    yield return DialogueManager.Instance.ShowDialogueText($"ʲôҲû�з�����");
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
            yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.PokemonBase.PokemonName}ϰ�����¼���\n{tmItem.Move.MoveName}��");
            _inventory.RemoveItem(tmItem);
        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.PokemonBase.PokemonName}��Ҫѧϰ{tmItem.Move.MoveName}...");
            yield return DialogueManager.Instance.ShowDialogueText($"����{pokemon.PokemonBase.PokemonName}���յļ���̫���ˣ�");

            yield return DialogueManager.Instance.ShowDialogueText($"��Ҫ��{pokemon.PokemonBase.PokemonName}\n�����ĸ����ܣ�", true, false);

            MoveToForgetState.I.NewMove = tmItem.Move;
            MoveToForgetState.I.CurrentMoves = pokemon.Moves.Select(m => m.MoveBase).ToList();
            yield return _gameManager.StateMachine.PushAndWait(MoveToForgetState.I);

            int moveIndex = MoveToForgetState.I.Selection;
            if (moveIndex == PokemonBase.MaxNumOfMoves || moveIndex == -1)
            {
                // Don't learn the new move
                yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.PokemonBase.PokemonName}����ѧϰ{tmItem.Move.MoveName}��");
            }
            else
            {
                // Forget the selected move and learn new move
                var selevtedMove = pokemon.Moves[moveIndex].MoveBase;
                yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.PokemonBase.PokemonName}������{selevtedMove.MoveName}��");
                pokemon.Moves[moveIndex] = new Move(tmItem.Move);
                _inventory.RemoveItem(tmItem);
            }

        }
    }

}

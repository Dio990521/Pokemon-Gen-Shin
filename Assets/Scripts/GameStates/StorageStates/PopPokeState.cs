using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopPokeState : State<GameManager>
{
    [SerializeField] private StorageListUI _storageListUI;
    public Image LeftArrow;

    public Pokemon SelectedPokemon { get; private set; }
    public static PopPokeState I { get; private set; }

    private GameManager _gameManager;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
        LeftArrow.color = Color.white;
        _storageListUI.OnSelected += OnPokemonSelected;
        _storageListUI.OnBack += OnBack;
    }

    public override void Execute()
    {
        _storageListUI.HandleUpdate();
    }

    public override void Exit(bool sfx = true)
    {
        _storageListUI.HideSelection();
        LeftArrow.color = Color.grey;
        _storageListUI.OnSelected -= OnPokemonSelected;
        _storageListUI.OnBack -= OnBack;
        StorageState.I.StorageUI.MessageText.text = "想要存放还是取出宝可梦？";
    }
    
    private void OnPokemonSelected(int selection)
    {
        if (!_storageListUI.IsCategoryEmpty())
        {
            StartCoroutine(ConfirmStorage(_gameManager.Storage.GetPokemon(selection, _storageListUI.SelectedCategory)));
        }
    }

    private IEnumerator ConfirmStorage(Pokemon selectedPokemon)
    {
        ChoiceState.I.Choices = new List<string>() { "取出", "查看" };
        yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

        int selectedChoice = ChoiceState.I.Selection;

        if (selectedChoice == 0)
        {
            if (_gameManager.PartyScreen.Pokemons.Count > 1)
            {
                _gameManager.Storage.PopPokemon(selectedPokemon);
                StorageState.I.StorageUI.MessageText.text = "想要存放还是取出宝可梦？";
            }
            else
            {
                StorageState.I.StorageUI.MessageText.text = "队伍已经满了！";
                yield return ConfirmStorage(selectedPokemon);
            }
        }
        else if (selectedChoice == 1)
        {
            PokemonInfoState.I.SelectedPokemon = selectedPokemon;
            yield return _gameManager.StateMachine.PushAndWait(PokemonInfoState.I);
            yield return ConfirmStorage(selectedPokemon);
        }

    }

    private void OnBack()
    {
        _gameManager.StateMachine.Pop();
    }
}

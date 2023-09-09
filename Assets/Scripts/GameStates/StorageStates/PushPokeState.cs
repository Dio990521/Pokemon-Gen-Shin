using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PushPokeState : State<GameManager>
{
    [SerializeField] private StoragePartyListUI _storagePartyUI;
    public Image RightArrow;

    public Pokemon SelectedPokemon { get; private set; }
    public static PushPokeState I { get; private set; }

    private GameManager _gameManager;

    private void Awake()
    {
        I = this;
    }


    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
        RightArrow.color = Color.white;
        _storagePartyUI.OnSelected += OnPokemonSelected;
        _storagePartyUI.OnBack += OnBack;
    }

    public override void Execute()
    {
        _storagePartyUI.HandleUpdate();
    }

    public override void Exit(bool sfx = true)
    {
        _storagePartyUI.HideSelection();
        _storagePartyUI.OnSelected -= OnPokemonSelected;
        _storagePartyUI.OnBack -= OnBack;
        RightArrow.color = Color.grey;
        StorageState.I.StorageUI.MessageText.text = "想要存放还是取出宝可梦？";
    }

    private void OnPokemonSelected(int selection)
    {
        StartCoroutine(ConfirmStorage(_storagePartyUI.Pokemons[selection]));
    }

    private IEnumerator ConfirmStorage(Pokemon selectedPokemon)
    {
        ChoiceState.I.Choices = new List<string>() { "存放", "查看" };
        yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

        int selectedChoice = ChoiceState.I.Selection;

        if (selectedChoice == 0)
        {
            if (_storagePartyUI.Pokemons.Count > 1)
            {
                _gameManager.Storage.PushPokemon(selectedPokemon);
                StorageState.I.StorageUI.MessageText.text = "想要存放还是取出宝可梦？";
            }
            else
            {
                StorageState.I.StorageUI.MessageText.text = "队伍中必须至少存在一个宝可梦！";
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

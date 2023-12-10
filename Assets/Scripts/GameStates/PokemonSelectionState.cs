using DG.Tweening;
using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonSelectionState : State<GameManager>
{
    [SerializeField] private PokemonSelectionUI _pokemonSelectionUI;
    public static PokemonSelectionState I { get; private set; }

    private GameManager _gameManager;

    //public Pokemon BossPokemon;

    [SerializeField] private GameObject npc;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
        _pokemonSelectionUI.OnSelected += OnPokemonSelected;
        _pokemonSelectionUI.OnBack += OnBack;
        _pokemonSelectionUI.Show();
    }

    public override void Execute()
    {
        _pokemonSelectionUI.HandleUpdate(false);
    }

    public override void Exit(bool sfx = true)
    {
        _pokemonSelectionUI.ResetSelection();
        _pokemonSelectionUI.OnSelected -= OnPokemonSelected;
        _pokemonSelectionUI.OnBack -= OnBack;
        _pokemonSelectionUI.Display.SetActive(false);
        _pokemonSelectionUI.gameObject.SetActive(false);
        var trainer = npc.GetComponent<TrainerController>();
        var party = npc.GetComponent<PokemonParty>().Pokemons;
        foreach (var pokemon in party)
        {
            pokemon.Init();
            pokemon.SetElementStatus(ConditionID.hydro, true);
        }
        BattleState.I.Guide = true;
        GameManager.Instance.StartTrainerBattle(trainer);
    }

    private void OnPokemonSelected(int selection)
    {
        var portrait = _pokemonSelectionUI.Display.transform.GetChild(0).GetComponent<Image>();
        portrait.sprite = _pokemonSelectionUI.SelectedPokemon.PokemonBase.FrontSprite;
        portrait.preserveAspect = true;
        _pokemonSelectionUI.Display.SetActive(true);
        StartDisplayAnim();
        _pokemonSelectionUI.Message.text = $"确定选择{_pokemonSelectionUI.SelectedPokemon.PokemonBase.PokemonName}吗？";
        StartCoroutine(HandleConfirm());
    }

    private void OnBack()
    {
        _gameManager.StateMachine.Pop();
    }

    private void StartDisplayAnim()
    {
        _pokemonSelectionUI.Display.transform.position = _pokemonSelectionUI.GetSelectedPokeballUI().transform.position;
        _pokemonSelectionUI.Display.transform.localScale = new Vector3(0f, 0f, 0f);
        Vector3 initialScale = new Vector3(0f, 0f, 0f);
        Vector3 targetScale = new Vector3(1f, 1f, 1f);
        float duration = 1f;

        var sequence = DOTween.Sequence();
        sequence.Append(_pokemonSelectionUI.Display.transform.DOScale(targetScale, duration)
            .From(initialScale)
            .SetEase(Ease.OutBack));
        sequence.Join(_pokemonSelectionUI.Display.transform.DOMove(_pokemonSelectionUI.EndPos, duration));

    }

    private IEnumerator HandleConfirm()
    {
        ChoiceState.I.Choices = new List<string>() { "就决定是你了！", "让我再看一看！" };
        yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

        int selectedChoice = ChoiceState.I.Selection;

        if (selectedChoice == 0)
        {
            _pokemonSelectionUI.SelectedPokemon.Init();
            PokemonParty.GetPlayerParty().AddPokemonToParty(_pokemonSelectionUI.SelectedPokemon);
            _gameManager.StateMachine.Pop();
        }
        else
        {
            _pokemonSelectionUI.Display.SetActive(false);
            _pokemonSelectionUI.Message.text = "派蒙遇到麻烦了！\n快选择一个宝可梦保护应急食物！";
        }
    }
}

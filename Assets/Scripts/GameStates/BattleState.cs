using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleState : State<GameManager> 
{
    [SerializeField] private BattleSystem _battleSystem;

    public BattleTrigger Trigger { get; set; }
    public TrainerController Trainer { get; set; }
    public static BattleState I { get; private set; }
    public BattleSystem BattleSystem { get => _battleSystem; set => _battleSystem = value; }

    private GameManager _gameManager;

    public Sprite GenshinPokeball;

    public Pokemon BossPokemon;
    public CutsceneName ActivateCutsceneAfterBattle;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
        _battleSystem.CreateBattleStateMachine();
        StartCoroutine(EnterBattle());
        _battleSystem.OnBattleOver += EndBattle;

    }

    public override void Exit(bool sfx = true)
    {
        _battleSystem.gameObject.SetActive(false);
        _gameManager.BattleTransitionManager.ClearTransition();
        _gameManager.WorldTransitionManager.ClearTransition();
        _gameManager.WorldCamera.gameObject.SetActive(true);
        _battleSystem.OnBattleOver -= EndBattle;
        if (ActivateCutsceneAfterBattle != CutsceneName.None)
        {
            GameKeyManager.Instance.SetBoolValue(ActivateCutsceneAfterBattle.ToString(), true);
        }
        ActivateCutsceneAfterBattle = CutsceneName.None;
        BossPokemon = null;
    }

    public override void Execute()
    {
        _battleSystem.HandleUpdate();
    }

    private IEnumerator EnterBattle()
    {

        PokemonParty playerParty = _gameManager.PlayerController.GetComponent<PokemonParty>();
        if (BossPokemon != null)
        {
            yield return EnterBattleTransition(TransitionType.WildBattle);
            _battleSystem.StartBattle(playerParty, BossPokemon, Trigger);
        }
        else if (Trainer == null)
        {
            yield return EnterBattleTransition(TransitionType.WildBattle);
            Pokemon wildPokemon = _gameManager.CurrentScene.GetComponent<MapArea>().GetRandomWildPokemon(Trigger);
            var wildPokemonCopy = new Pokemon(wildPokemon.PokemonBase, wildPokemon.Level);
            _battleSystem.StartBattle(playerParty, wildPokemonCopy, Trigger);
        }
        else
        {
            yield return EnterBattleTransition(TransitionType.TrainerBattle);
            PokemonParty trainerParty = Trainer.GetComponent<PokemonParty>();
            _battleSystem.StartTrainerBattle(playerParty, trainerParty, Trainer.BattleTrigger);
        }
    }

    private IEnumerator EnterBattleTransition(TransitionType transitionType)
    {
        yield return _gameManager.WorldTransitionManager.StartTransition(transitionType, 2f);
        yield return new WaitForSeconds(2f);
        yield return _gameManager.BattleTransitionManager.StartTransition(TransitionType.TopBottom, 1.5f);
        _battleSystem.gameObject.SetActive(true);
        _gameManager.WorldCamera.gameObject.SetActive(false);
    }

    private void EndBattle(bool won)
    {
        if (Trainer != null && won)
        {
            Trainer.BattleLost();
            Trainer = null;
        }
        StartCoroutine(Fader.FadeOut(1f));
        _gameManager.StateMachine.Pop();

    }

}

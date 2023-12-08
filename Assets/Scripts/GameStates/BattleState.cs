using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossType { Gongzi, Nvshi, Fengmo, Ruotuo, jiangui, leijun, cao, langwang, none }


public class BattleState : State<GameManager> 
{
    [SerializeField] private BattleSystem _battleSystem;

    public BattleTrigger Trigger { get; set; }

    public BossType BossType { get; set; }
    public TrainerController Trainer { get; set; }
    public static BattleState I { get; private set; }
    public BattleSystem BattleSystem { get => _battleSystem; set => _battleSystem = value; }

    private GameManager _gameManager;

    public Sprite GenshinPokeball;

    public Pokemon BossPokemon;
    public bool IsSuperBoss;
    public CutsceneName ActivateCutsceneAfterBattle;

    [HideInInspector]
    public bool Guide = false;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
        _battleSystem.CreateBattleStateMachine();
        StartCoroutine(EnterBattle());
        BattleSystem.OnBattleOver += EndBattle;

    }

    public override void Exit(bool sfx = true)
    {
        StartCoroutine(DisableBattleCanvas());
        BattleSystem.OnBattleOver -= EndBattle;
        BossPokemon = null;
        BossType = BossType.none;
        IsSuperBoss = false;
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
            if (IsSuperBoss)
            {
                yield return EnterBattleTransition(TransitionType.SuperBossBattle);
            }
            else
            {
                yield return EnterBattleTransition(TransitionType.WildBattle);
            }
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
            if (Trainer.IsBoss)
            {
                yield return EnterBattleTransition(TransitionType.BossBattle);
            }
            else
            {
                yield return EnterBattleTransition(TransitionType.TrainerBattle);
            }
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
        _gameManager.StateMachine.Pop();
    }

    private IEnumerator DisableBattleCanvas()
    {
        GameManager.Instance.PauseGame(true);
        var playerParty = _battleSystem.PlayerParty;
        bool hasEvolutions = playerParty.CheckForEvolutions();
        yield return Fader.FadeIn(1f);
        if (!hasEvolutions)
        {
            AudioManager.Instance.PlayMusicVolume(GameManager.Instance.CurrentScene.SceneMusic, fade: true);
        }
        BattleSystem.PartyScreen.SetPartyData();
        _battleSystem.gameObject.SetActive(false);
        _gameManager.BattleTransitionManager.ClearTransition();
        _gameManager.WorldTransitionManager.ClearTransition();
        _gameManager.WorldCamera.gameObject.SetActive(true);
        if (ActivateCutsceneAfterBattle != CutsceneName.None)
        {
            GameKeyManager.Instance.SetBoolValue(ActivateCutsceneAfterBattle.ToString(), true);
        }
        ActivateCutsceneAfterBattle = CutsceneName.None;
        yield return new WaitForSeconds(1f);
        GameManager.Instance.PauseGame(false);
        yield return Fader.FadeOut(1f);

        
        if (hasEvolutions)
        {
            yield return playerParty.RunEvolutions();
        }

    }

}

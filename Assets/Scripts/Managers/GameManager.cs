using PokeGenshinUtils.StateMachine;
using System;
using System.Collections;
using UnityEngine;
using Game.Tool.Singleton;
using System.Collections.Generic;
using UnityEditor;

public class GameManager : Singleton<GameManager>, ISavable
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private PartyScreen partyScreen;
    [SerializeField] private PokemonInfoUI pokemonInfoUI;
    [SerializeField] private PartyMenu partyMenu;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private SaveLoadUI saveLoadUI;
    [SerializeField] private AchievementUI achievementUI;
    [SerializeField] private PokemonSelectionUI _pokesSelectionUI;
    [SerializeField] private StoragePartyListUI _storagePartyListUI;
    [SerializeField] private StorageListUI _storageListUI;
    [SerializeField] private RouteIcon routeIcon;
    [SerializeField] private TransitionManager _worldTransitionManager;
    [SerializeField] private TransitionManager _battleTransitionManager;
    [SerializeField] private Storage _storage;
    public GameObject TitleUI;
    public GameObject EndingUI;

    public List<GameObject> Managers;

    public List<Sprite> PokeballSprites;

    public StateMachine<GameManager> StateMachine { get; private set; }

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }
    public PlayerController PlayerController { get => playerController;}

    public RouteIcon RouteIcon { get => routeIcon; }
    public Camera WorldCamera { get => worldCamera; set => worldCamera = value; }
    public TransitionManager WorldTransitionManager { get => _worldTransitionManager; set => _worldTransitionManager = value; }
    public TransitionManager BattleTransitionManager { get => _battleTransitionManager; set => _battleTransitionManager = value; }
    public PartyScreen PartyScreen { get => partyScreen; set => partyScreen = value; }
    public Storage Storage { get => _storage; set => _storage = value; }
    public BattleSystem BattleSystem { get => battleSystem; set => battleSystem = value; }

    private float _gameTimeSpend;
    public string GamePlayTime;

    protected override void Awake()
    {
        base.Awake();
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        ConditionsDB.Init();
        PokemonDB.Init();
        MoveDB.Init();
        ItemDB.Init();
        QuestDB.Init();
        PassiveMoveDB.Init();
        NewGameInit();
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        //menuController = GetComponent<MenuController>();
    }

    private void Start()
    {
        StateMachine.ChangeState(FreeRoamState.I);
        DialogueManager.Instance.OnShowDialogue += () =>
        {
            StateMachine.Push(DialogueState.I);
        };

        DialogueManager.Instance.OnDialogueFinished += () =>
        {
            StateMachine.Pop();
        };
    }

    public void NewGameInit()
    {
        StateMachine = new StateMachine<GameManager>(this);
        _storage.Init();
        _gameTimeSpend = 0f;
        _worldTransitionManager.ClearTransition(true);
        _battleTransitionManager.ClearTransition(true);
        partyScreen.Init();
        _storagePartyListUI.Init();
        _storageListUI.Init();
        SavingSystem.I.ClearSavingData();
        foreach (var manager in Managers)
        {
            manager.GetComponent<IManager>().Init();
        }
    }

    public void NewGame()
    {
        StartCoroutine(NewGameTransition());
    }

    public IEnumerator NewGameTransition()
    {
        yield return Fader.FadeIn(0.5f);
        NewGameInit();
        StateMachine.ChangeState(FreeRoamState.I);
        //PauseGame(false);
        playerController.transform.localPosition = new Vector3(-8.5f, -22.35f);
        TitleUI.SetActive(false);
        PlayerController.Character.Animator.SetFacingDirection(FacingDirection.Down);
        yield return new WaitForSeconds(0.5f);
        yield return Fader.FadeOut(0.5f);
    }

    public void GoToEnding()
    {
        StartCoroutine(GoToEndingTransition());
    }

    public IEnumerator GoToEndingTransition()
    {
        PauseGame(true);
        yield return Fader.FadeIn(0.5f);
        playerController.transform.localPosition = new Vector3(-207.5f, 97.3f);
        EndingUI.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        yield return Fader.FadeOut(0.5f);
    }

    public void OpenLoadScene()
    {
        StateMachine.Push(LoadState.I);
    }

    public void BackToTitle()
    {
        StartCoroutine(BackToTitleTransition());
    }

    public IEnumerator BackToTitleTransition()
    {
        PauseGame(true);
        yield return Fader.FadeIn(0.5f);
        playerController.transform.localPosition = new Vector3(-178.5f, 171.3f);
        EndingUI.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        yield return Fader.FadeOut(0.5f);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            if (StateMachine.CurrentState != PauseState.I)
            {
                StateMachine.Push(PauseState.I);
            }
        }
        else
        {
            StateMachine.Pop();
        }
    }

    public void StartBattle(BattleTrigger trigger)
    {
        AudioManager.Instance.PlayMusicVolume(BGM.BATTLE_WILD_POKEMON);
        BattleState.I.Trigger = trigger;
        BattleState.I.IsSuperBoss = false;
        BattleState.I.BossPokemon = null;
        StateMachine.Push(BattleState.I);
    }

    public void StartBossBattle(BattleTrigger trigger, Pokemon selectedPokemon, BossType bossType, 
        CutsceneName activateCutsceneAfterBattle = CutsceneName.None, bool isSuperBoss = false)
    {
        if (!isSuperBoss)
        {
            AudioManager.Instance.PlayMusicVolume(BGM.BATTLE_WILD_POKEMON);
        }
        else
        {
            AudioManager.Instance.PlayMusicVolume(BGM.BATTLE_SUPER_ANCIENT_POKEMON);
        }
        BattleState.I.IsSuperBoss = isSuperBoss;
        BattleState.I.Trigger = trigger;
        BattleState.I.BossPokemon = selectedPokemon;
        selectedPokemon.PokeballSpriteType = PokeballType.Guaishou;
        BattleState.I.BossPokemon.Init();
        if (activateCutsceneAfterBattle != CutsceneName.None)
        {
            BattleState.I.ActivateCutsceneAfterBattle = activateCutsceneAfterBattle;
        }
        StateMachine.Push(BattleState.I);
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        AudioManager.Instance.PlayMusicVolume(trainer.StartBGM);
        BattleState.I.Trainer = trainer;
        BattleState.I.BossPokemon = null;
        StateMachine.Push(BattleState.I);
    }

    public void OnEnterTrainersView(TrainerController trainer)
    {
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    }

    private void Update()
    {
        _gameTimeSpend += Time.deltaTime;

        // 将秒数转换为小时和分钟
        int hours = (int)(_gameTimeSpend / 3600);
        int minutes = ((int)_gameTimeSpend - hours * 3600) / 60;

        // 格式化为“小时:分钟”的字符串
        GamePlayTime = string.Format("{0:00}:{1:00}", hours, minutes);
        StateMachine.Execute();

    }

    public IEnumerator MoveCamera(Vector2 moveOffset, bool waitForFadeOut=false)
    {
        yield return Fader.FadeIn(0.5f);
        worldCamera.transform.position += new Vector3(moveOffset.x, moveOffset.y);

        if (waitForFadeOut)
        {
            yield return Fader.FadeOut(0.5f);
        }
        else
        {
            StartCoroutine(Fader.FadeOut(0.5f));
        }
        
    }

    public void SetCurrentScene(SceneDetails curScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = curScene;
    }

    public IEnumerator LoadGame(string fileName)
    {
        PauseGame(true);
        SavingSystem.I.Load(fileName);
        CurrentScene.UnloadScene();
        CurrentScene.UnloadConnectedScenes();
        yield return new WaitForSeconds(1.5f);
        yield return CurrentScene.LoadScene();
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator RefreshScene()
    {
        PauseGame(true);
        CurrentScene.ForceUnloadConnectedScenes();
        yield return new WaitForSeconds(1f);
        CurrentScene.LoadConnectedScenes();
        yield return new WaitForSeconds(1f);
    }

    public object CaptureState()
    {
        return _gameTimeSpend;
    }

    public void RestoreState(object state)
    {
        _gameTimeSpend = (float)state;
    }

    private void OnGUI()
    {
        if (StateMachine == null) return;
        var style = new GUIStyle();
        style.fontSize = 25;
        GUILayout.Label("STATE STACK", style);
        foreach (var state in StateMachine.StateStack)
        {
            GUILayout.Label(state.GetType().ToString(), style);
        }
    }

    public Sprite GetPokeSprite(PokeballType type)
    {
        return PokeballSprites[(int)type];
    }
}

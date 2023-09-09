using PokeGenshinUtils.StateMachine;
using System;
using System.Collections;
using UnityEngine;

//public enum GameState 
//{ 
//    FreeRoam, Battle, Dialogue, Menu, Bag, Shop, PartyScreen, Cutscene, Pause, Evolution, Computer, 
//    PokeInfo, Save, Load, PartyMenu, PokemonSwitch, Achievement, PokemonSelection
//}

public class GameManager : Game.Tool.Singleton.Singleton<GameManager>, ISavable
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

    private TrainerController trainer;

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

    private float _gameTimeSpend;
    public string GamePlayTime;

    private int switchPokemon;

    protected override void Awake()
    {
        base.Awake();
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        ConditionsDB.Init();
        PokemonDB.Init();
        MoveDB.Init();
        ItemDB.Init();
        QuestDB.Init();
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        //menuController = GetComponent<MenuController>();
    }

    private void Start()
    {
        StateMachine = new StateMachine<GameManager>(this);
        StateMachine.ChangeState(FreeRoamState.i);

        _gameTimeSpend = 0f;
        _worldTransitionManager.ClearTransition(true);
        _battleTransitionManager.ClearTransition(true);

        battleSystem.OnBattleOver += EndBattle;

        partyScreen.Init();
        _storagePartyListUI.Init();
        _storageListUI.Init();
        DialogueManager.Instance.OnShowDialogue += () =>
        {
            StateMachine.Push(DialogueState.I);
        };

        DialogueManager.Instance.OnDialogueFinished += () =>
        {
            StateMachine.Pop();
        };

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
        AudioManager.Instance.PlayMusic(BGM.BATTLE_WILD_POKEMON);
        BattleState.I.Trigger = trigger;
        StateMachine.Push(BattleState.I);
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        AudioManager.Instance.PlayMusic(BGM.BATTLE_TRAINER);
        BattleState.I.Trainer = trainer;
        StateMachine.Push(BattleState.I);
    }

    public void OnEnterTrainersView(TrainerController trainer)
    {
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    }

    private void EndBattle(bool won)
    {
        if (trainer != null && won)
        {
            trainer.BattleLost();
            trainer = null;
        }

        partyScreen.SetPartyData();

        battleSystem.gameObject.SetActive(false);
        _battleTransitionManager.ClearTransition();
        _worldTransitionManager.ClearTransition();
        worldCamera.gameObject.SetActive(true);

        var playerParty = playerController.GetComponent<PokemonParty>();

        bool hasEvolutions = playerParty.CheckForEvolutions();
        if (hasEvolutions)
        {
            StartCoroutine(playerParty.RunEvolutions());
        }
        else
        {
            AudioManager.Instance.PlayMusic(CurrentScene.SceneMusic, fade: true);
        }
        
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
        SavingSystem.i.Load(fileName);
        CurrentScene.UnloadScene();
        CurrentScene.UnloadConnectedScenes();
        yield return new WaitForSeconds(1.5f);
        yield return CurrentScene.LoadScene();
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
        var style = new GUIStyle();
        style.fontSize = 25;
        GUILayout.Label("STATE STACK", style);
        foreach (var state in StateMachine.StateStack)
        {
            GUILayout.Label(state.GetType().ToString(), style);
        }
    }
}

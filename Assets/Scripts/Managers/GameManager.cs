using PokeGenshinUtils.StateMachine;
using System;
using System.Collections;
using UnityEngine;

public enum GameState 
{ 
    FreeRoam, Battle, Dialogue, Menu, Bag, Shop, PartyScreen, Cutscene, Pause, Evolution, Computer, 
    PokeInfo, Save, Load, PartyMenu, PokemonSwitch, Achievement, PokemonSelection
}

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
    [SerializeField] private RouteIcon routeIcon;
    [SerializeField] private TransitionManager _worldTransitionManager;
    [SerializeField] private TransitionManager _battleTransitionManager;


    private GameState state;
    public GameState prevState;
    public GameState stateBeforeEvolution;
    private TrainerController trainer;

    public StateMachine<GameManager> StateMachine { get; private set; }

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }
    public GameState State { get => state; set => state = value; }
    public PlayerController PlayerController { get => playerController;}

    public RouteIcon RouteIcon { get => routeIcon; }
    public Camera WorldCamera { get => worldCamera; set => worldCamera = value; }
    public TransitionManager WorldTransitionManager { get => _worldTransitionManager; set => _worldTransitionManager = value; }
    public TransitionManager BattleTransitionManager { get => _battleTransitionManager; set => _battleTransitionManager = value; }
    public PartyScreen PartyScreen { get => partyScreen; set => partyScreen = value; }

    //MenuController menuController;

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

        DialogueManager.Instance.OnShowDialogue += () =>
        {
            StateMachine.Push(DialogueState.I);
        };

        DialogueManager.Instance.OnDialogueFinished += () =>
        {
            StateMachine.Pop();
        };

        //menuController.OnBack += () =>
        //{
        //    State = GameState.FreeRoam;
        //};

        //menuController.OnMenuSelected += MenuSelected;
        partyMenu.OnMenuSelected += PartyMenuSelected;
        partyMenu.OnBack += () =>
        {
            State = GameState.PartyScreen;
        };

        ComputerController.Instance.OnStart += () => state = GameState.Computer;
        ComputerController.Instance.OnFinish += () => state = GameState.FreeRoam;
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
        //
        //State = GameState.Battle;
        //yield return _worldTransitionManager.StartTransition(TransitionType.WildBattle, 2f);
        //yield return new WaitForSeconds(2f);
        //yield return _battleTransitionManager.StartTransition(TransitionType.TopBottom, 1.5f);
        //battleSystem.gameObject.SetActive(true);
        //worldCamera.gameObject.SetActive(false);

        //PokemonParty playerParty = playerController.GetComponent<PokemonParty>();
        //Pokemon wildPokemon = CurrentScene.GetComponent<MapArea>().GetRandomWildPokemon(trigger);

        //var wildPokemonCopy = new Pokemon(wildPokemon.PokemonBase, wildPokemon.Level);
        //battleSystem.StartBattle(playerParty, wildPokemonCopy, trigger);
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        AudioManager.Instance.PlayMusic(BGM.BATTLE_TRAINER);
        BattleState.I.Trainer = trainer;
        StateMachine.Push(BattleState.I);
        //State = GameState.Battle;
        //yield return _worldTransitionManager.StartTransition(TransitionType.TrainerBattle, 2f);
        //yield return new WaitForSeconds(2f);
        //yield return _battleTransitionManager.StartTransition(TransitionType.TopBottom, 1.5f);
        //battleSystem.gameObject.SetActive(true);
        //worldCamera.gameObject.SetActive(false);
        //this.trainer = trainer;
        //PokemonParty playerParty = playerController.GetComponent<PokemonParty>();
        //PokemonParty trainerParty = trainer.GetComponent<PokemonParty>();

        //battleSystem.StartTrainerBattle(playerParty, trainerParty);
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

        State = GameState.FreeRoam;
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

        if (State == GameState.Computer)
        {
            ComputerController.Instance.HandleUpdate();
        }
        else if (State == GameState.PokeInfo)
        {
            pokemonInfoUI.HandleUpdate();
        }
        else if (State == GameState.Save)
        {
            saveLoadUI.HandleUpdate(save: true);
        }
        else if (State == GameState.Load)
        {
            saveLoadUI.HandleUpdate(save: false);
        }
        //else if (State == GameState.PartyMenu)
        //{
        //    partyMenu.HandleUpdate();
        //}
        else if (State == GameState.Achievement)
        {
            achievementUI.HandleUpdate();
        }
        //else if (State == GameState.PokemonSelection)
        //{
        //    _pokesSelectionUI.HandleUpdate();
        //}

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

    private void MenuSelected(int selectedItem)
    {
        AudioManager.Instance.PlaySE(SFX.CONFIRM);
        //if (selectedItem == 0)
        //{
        //    // Pokemon
        //    partyScreen.Show();
        //    State = GameState.PartyScreen;
        //} 
        //else if (selectedItem == 1)
        //{
        //    // Bag
        //    inventoryUI.Show();
        //    State = GameState.Bag;

        //}
        if (selectedItem == 2)
        {
            // Save
            saveLoadUI.Show();
            State = GameState.Save;

        }
        else if (selectedItem == 3)
        {
            // Load
            saveLoadUI.Show();
            State = GameState.Load;
        }
        else if (selectedItem == 4)
        {
            // Achievement
            achievementUI.Show();
            State = GameState.Achievement;
        }
        else if (selectedItem == 5)
        {
            // Setting
            _pokesSelectionUI.Show();
            State = GameState.PokemonSelection;
        }
        else if (selectedItem == 6)
        {
            // Title
        }
    }

    private void PartyMenuSelected(int selectedItem, Pokemon selectedPokemon)
    {
        //AudioManager.Instance.PlaySE(SFX.CONFIRM);
        if (selectedItem == 0)
        {
            // Pokemon Summary
            pokemonInfoUI.Show(selectedPokemon);
            State = GameState.PokeInfo;
        }
        else if (selectedItem == 1)
        {
            // Switch Pokemon
            //switchPokemon = partyScreen.Selection;
            partyScreen.SetMessageText("选择要交换的宝可梦。");
            partyMenu.Close();
            State = GameState.PokemonSwitch;

        }
    }

    public IEnumerator LoadGame(string fileName)
    {
        PauseGame(true);
        yield return Fader.FadeIn(1f);
        saveLoadUI.Close();
        SavingSystem.i.Load(fileName);
        CurrentScene.UnloadScene();
        CurrentScene.UnloadConnectedScenes();
        yield return new WaitForSeconds(1.5f);
        yield return CurrentScene.LoadScene();
        yield return new WaitForSeconds(1f);
        yield return Fader.FadeOut(1f);
        PauseGame(false);
        State = GameState.FreeRoam;
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

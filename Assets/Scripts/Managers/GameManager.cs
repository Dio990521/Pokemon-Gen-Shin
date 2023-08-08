using Game.Tool.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState { FreeRoam, Battle, Dialogue, Menu, Bag, Shop, PartyScreen, Cutscene, Pause, Evolution, Computer, PokeInfo, Save, Load, PartyMenu, PokemonSwitch }

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
    [SerializeField] private RouteIcon routeIcon;
    [SerializeField] private TransitionManager _worldTransitionManager;
    [SerializeField] private TransitionManager _battleTransitionManager;


    private GameState state;
    public GameState prevState;
    public GameState stateBeforeEvolution;
    private TrainerController trainer;

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }
    public GameState State { get => state; set => state = value; }
    public PlayerController PlayerController { get => playerController;}

    public RouteIcon RouteIcon { get => routeIcon; }

    MenuController menuController;

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
        menuController = GetComponent<MenuController>();
    }

    private void Start()
    {
        _gameTimeSpend = 0f;
        _worldTransitionManager.ClearTransition(true);
        _battleTransitionManager.ClearTransition(true);

        battleSystem.OnBattleOver += EndBattle;

        partyScreen.Init();

        DialogueManager.Instance.OnShowDialogue += () =>
        {
            prevState = State;
            State = GameState.Dialogue;
        };

        DialogueManager.Instance.OnDialogueFinished += () =>
        {
            if (State == GameState.Dialogue)
            {
                State = prevState;
            }
        };

        menuController.OnBack += () =>
        {
            State = GameState.FreeRoam;
        };

        menuController.OnMenuSelected += MenuSelected;
        partyMenu.OnMenuSelected += PartyMenuSelected;
        partyMenu.OnBack += () =>
        {
            State = GameState.PartyScreen;
        };

        EvolutionManager.Instance.OnStartEvolution += () =>
        {
            stateBeforeEvolution = State;
            State = GameState.Evolution;
        };

        EvolutionManager.Instance.OnCompleteEvolution += () =>
        {
            State = stateBeforeEvolution;
            partyScreen.SetPartyData();

            AudioManager.Instance.PlayMusic(CurrentScene.SceneMusic, fade: true);
        };

        ShopController.Instance.OnStart += () => state = GameState.Shop;
        ShopController.Instance.OnFinish += () => state = GameState.FreeRoam;
        ComputerController.Instance.OnStart += () => state = GameState.Computer;
        ComputerController.Instance.OnFinish += () => state = GameState.FreeRoam;
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            prevState = State;
            State = GameState.Pause;
        }
        else
        {
            State = prevState;
        }
    }

    public void StartCutsceneState()
    {
        state = GameState.Cutscene;
    }

    public void StartFreeRoamState()
    {
        state = GameState.FreeRoam;
    }

    public IEnumerator StartBattle(BattleTrigger trigger)
    {
        AudioManager.Instance.PlayMusic(BGM.BATTLE_WILD_POKEMON);
        State = GameState.Battle;
        yield return _worldTransitionManager.StartTransition(TransitionType.WildBattle, 2f);
        yield return new WaitForSeconds(2f);
        yield return _battleTransitionManager.StartTransition(TransitionType.TopBottom, 1.5f);
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        PokemonParty playerParty = playerController.GetComponent<PokemonParty>();
        Pokemon wildPokemon = CurrentScene.GetComponent<MapArea>().GetRandomWildPokemon(trigger);

        var wildPokemonCopy = new Pokemon(wildPokemon.PokemonBase, wildPokemon.Level);
        battleSystem.StartBattle(playerParty, wildPokemonCopy, trigger);
    }

    public IEnumerator StartTrainerBattle(TrainerController trainer)
    {
        AudioManager.Instance.PlayMusic(BGM.BATTLE_TRAINER);
        State = GameState.Battle;
        yield return _worldTransitionManager.StartTransition(TransitionType.TrainerBattle, 2f);
        yield return new WaitForSeconds(2f);
        yield return _battleTransitionManager.StartTransition(TransitionType.TopBottom, 1.5f);
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        this.trainer = trainer;
        PokemonParty playerParty = playerController.GetComponent<PokemonParty>();
        PokemonParty trainerParty = trainer.GetComponent<PokemonParty>();

        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }

    public void OnEnterTrainersView(TrainerController trainer)
    {
        State = GameState.Cutscene;
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

        if (State == GameState.FreeRoam)
        {
            playerController.HandleUpdate();

            if (Input.GetKeyDown(KeyCode.Return))
            {
                menuController.OpenMenu();
                State = GameState.Menu;
            }
        }
        else if (State == GameState.Cutscene)
        {
            playerController.Character.HandleUpdate();
        }
        else if (State == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (State == GameState.Dialogue)
        {
            DialogueManager.Instance.HandleUpdate();
        }
        else if (State == GameState.Menu)
        {
            menuController.HandleUpdate();
        }
        else if (State == GameState.PartyScreen)
        {

            Action onSelected = () =>
            {
                partyMenu.Show(partyScreen.SelectedMember);
                State = GameState.PartyMenu;
            };

            Action onBack = () =>
            {
                partyScreen.gameObject.SetActive(false);
                State = GameState.FreeRoam;
            };

            partyScreen.HandleUpdate(onSelected, onBack);
        }
        else if (State == GameState.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                State = GameState.FreeRoam;
            };

            inventoryUI.HandleUpdate(onBack);
        }
        else if (State == GameState.Shop)
        {
            ShopController.Instance.HandleUpdate();
        }
        else if (State == GameState.Computer)
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
        else if (State == GameState.PartyMenu)
        {
            partyMenu.HandleUpdate();
        }
        else if (State == GameState.PokemonSwitch)
        {
            Action onSelected = () =>
            {
                partyScreen.SwitchPokemonSlot(switchPokemon, partyScreen.Selection);
                State = GameState.PartyScreen;
            };

            Action onBack = () =>
            {
                partyScreen.SetMessageText("选择一个宝可梦。");
                State = GameState.PartyScreen;
            };

            partyScreen.HandleUpdate(onSelected, onBack);
        }

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
        if (selectedItem == 0)
        {
            // Pokemon
            partyScreen.Show();
            State = GameState.PartyScreen;
        } 
        else if (selectedItem == 1)
        {
            // Bag
            inventoryUI.Show();
            State = GameState.Bag;

        }
        else if (selectedItem == 2)
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
    }

    private void PartyMenuSelected(int selectedItem, Pokemon selectedPokemon)
    {
        AudioManager.Instance.PlaySE(SFX.CONFIRM);
        if (selectedItem == 0)
        {
            // Pokemon Summary
            pokemonInfoUI.Show(selectedPokemon);
            State = GameState.PokeInfo;
        }
        else if (selectedItem == 1)
        {
            // Switch Pokemon
            switchPokemon = partyScreen.Selection;
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
}

using Game.Tool.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialogue, Menu, Bag, Shop, PartyScreen, Cutscene, Pause, Evolution }

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private PartyScreen partyScreen;
    [SerializeField] private InventoryUI inventoryUI;

    private GameState state;
    public GameState prevState;
    public GameState stateBeforeEvolution;
    private TrainerController trainer;


    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }
    public GameState State { get => state; set => state = value; }
    public PlayerController PlayerController { get => playerController;}

    MenuController menuController;

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

    public void StartBattle(BattleTrigger trigger)
    {
        AudioManager.Instance.PlayMusic(BGM.BATTLE_WILD_POKEMON);
        State = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        PokemonParty playerParty = playerController.GetComponent<PokemonParty>();
        Pokemon wildPokemon = CurrentScene.GetComponent<MapArea>().GetRandomWildPokemon(trigger);

        var wildPokemonCopy = new Pokemon(wildPokemon.PokemonBase, wildPokemon.Level);
        battleSystem.StartBattle(playerParty, wildPokemonCopy, trigger);
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        AudioManager.Instance.PlayMusic(BGM.BATTLE_TRAINER);
        State = GameState.Battle;
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
                // Summary Screen
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
            partyScreen.gameObject.SetActive(true);
            State = GameState.PartyScreen;
        } 
        else if (selectedItem == 1)
        {
            // Bag
            inventoryUI.gameObject.SetActive(true);
            State = GameState.Bag;

        }
        else if (selectedItem == 2)
        {
            // Save
            SavingSystem.i.Save("saveSlot1");
            State = GameState.FreeRoam;
        }
        else if (selectedItem == 3)
        {
            // Load
            SavingSystem.i.Load("saveSlot1");
            State = GameState.FreeRoam;
        }

        
    }
}

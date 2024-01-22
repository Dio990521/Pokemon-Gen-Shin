using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuState : State<GameManager>
{
    [SerializeField] private MenuController _menuController;
    public static GameMenuState I { get; private set; }

    private GameManager _gameManager;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(GameManager owner)
    {
        AudioManager.Instance.PlaySE(SFX.MENU);
        _gameManager = owner;
        _menuController.gameObject.SetActive(true);
        _menuController.OnSelected += OnMenuItemSelected;
        _menuController.OnBack += OnBack;
    }

    public override void Execute()
    {
        _menuController.HandleUpdate();
    }

    public override void Exit(bool sfx)
    {
        _menuController.ResetSelection();
        _menuController.OnSelected -= OnMenuItemSelected;
        _menuController.OnBack -= OnBack;
        _menuController.gameObject.SetActive(false);
    }

    private void OnMenuItemSelected(int selection)
    {
        if ( selection == 0 )
        {
            // Party Screen
            _gameManager.StateMachine.Push(PartyState.I);
        }
        else if (selection == 1 )
        {
            // Inventory Screen
            _gameManager.StateMachine.Push(InventoryState.I);
        }
        else if (selection == 2)
        {
            // Save Screen
            _gameManager.StateMachine.Push(SaveState.I);
        }
        else if (selection == 3)
        {
            // Load Screen
            _gameManager.StateMachine.Push(LoadState.I);
        }
        else if (selection == 4)
        {
            // Achievement Screen
            _gameManager.StateMachine.Push(AchievementState.I);
        }
        else if (selection == 5)
        {
            // Setting Screen
#if UNITY_EDITOR
            _gameManager.GameMaster.gameObject.SetActive(true);
#endif
            _gameManager.StateMachine.Push(OptionState.I);
        }
        else if (selection == 6)
        {
            // Title Screen
            _gameManager.StateMachine.Pop();
            _gameManager.BackToTitle();
        }
    }

    private void OnBack()
    {
        _gameManager.StateMachine.Pop();
    }
}

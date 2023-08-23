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
        //AudioManager.Instance.PlaySE(SFX.CANCEL);
        _menuController.ResetSelection();
        _menuController.OnSelected -= OnMenuItemSelected;
        _menuController.OnBack -= OnBack;
        _menuController.gameObject.SetActive(false);
    }

    private void OnMenuItemSelected(int selection)
    {
        print($"shit!fuck {selection} you!");
        if ( selection == 0 )
        {
            _gameManager.StateMachine.Push(PartyState.I);
        }
        else if (selection == 1 )
        {
            _gameManager.StateMachine.Push(InventoryState.I);
        }
    }

    private void OnBack()
    {
        _gameManager.StateMachine.Pop();
    }
}

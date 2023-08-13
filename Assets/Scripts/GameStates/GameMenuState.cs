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
    }

    public override void Execute()
    {
        _menuController.HandleUpdate();
        if (Input.GetKeyDown(KeyCode.X))
        {
            _gameManager.StateMachine.Pop();
        }
    }

    public override void Exit()
    {
        AudioManager.Instance.PlaySE(SFX.CANCEL);
        _menuController.ResetSelection();
        _menuController.gameObject.SetActive(false);
    }
}

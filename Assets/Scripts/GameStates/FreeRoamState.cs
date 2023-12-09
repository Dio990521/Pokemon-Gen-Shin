using Game.Tool.Singleton;
using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeRoamState : State<GameManager>
{
    public static FreeRoamState I { get; private set; }

    private void Awake()
    {
        I = this;
    }

    private GameManager _gameManager;

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
    }

    public override void Execute()
    {
        PlayerController.I.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameManager.Instance.PlayerController.StopMovingAnimation();
            _gameManager.StateMachine.Push(GameMenuState.I);
        }
    }
}

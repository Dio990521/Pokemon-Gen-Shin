using Game.Tool.Singleton;
using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeRoamState : State<GameManager>
{
    public static FreeRoamState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    private GameManager _gameManager;

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
    }

    public override void Execute()
    {
        PlayerController.i.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            _gameManager.StateMachine.Push(GameMenuState.i);
        }
        //    {
        //        menuController.OpenMenu();
        //        State = GameState.Menu;
        //    }
    }
}

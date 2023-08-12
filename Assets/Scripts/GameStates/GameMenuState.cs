using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuState : State<GameManager>
{

    public static GameMenuState i { get; private set; }

    private GameManager _gameManager;

    private void Awake()
    {
        i = this;
    }

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
    }

    public override void Execute()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            _gameManager.StateMachine.Pop();
        }
    }
}

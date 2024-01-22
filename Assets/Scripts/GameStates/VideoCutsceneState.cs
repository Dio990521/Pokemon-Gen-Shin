using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoCutsceneState : State<GameManager>
{
    public static VideoCutsceneState I { get; private set; }

    private GameManager _gameManager;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
    }

    // Update is called once per frame
    public override void Execute()
    {
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X) ||
            Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
        {
            VideoManager.Instance.Skip();
        }
    }
}

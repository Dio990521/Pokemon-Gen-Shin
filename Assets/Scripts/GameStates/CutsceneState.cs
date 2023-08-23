using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneState : State<GameManager>
{
    public static CutsceneState I { get; private set; }

    private void Awake()
    {
        I = this;
    }

    public override void Execute()
    {
        PlayerController.I.Character.HandleUpdate();
    }
}

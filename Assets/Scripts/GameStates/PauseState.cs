using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseState : State<GameManager>
{
    public static PauseState I { get; private set; }


    private void Awake()
    {
        I = this;
    }
}

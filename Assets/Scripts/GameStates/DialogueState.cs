using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueState : State<GameManager>
{
    public static DialogueState I { get; private set; }

    private void Awake()
    {
        I = this;
    }
}
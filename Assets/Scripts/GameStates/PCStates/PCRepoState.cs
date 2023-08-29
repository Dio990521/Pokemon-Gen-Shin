using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCRepoState : State<GameManager>
{
    public static PCRepoState I { get; private set; }

    private GameManager _gameManager;

    private void Awake()
    {
        I = this;
    }
}

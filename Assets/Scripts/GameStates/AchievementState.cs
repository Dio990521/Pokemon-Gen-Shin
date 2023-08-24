using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementState : State<GameManager>
{
    [SerializeField] private AchievementUI _achievementUI;

    public static AchievementState I { get; private set; }

    private GameManager _gameManager;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
        _achievementUI.Show();
    }

    public override void Execute()
    {
        _achievementUI.HandleUpdate();
    }


}

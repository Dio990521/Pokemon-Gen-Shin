using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveState : State<GameManager>
{
    [SerializeField] private SaveLoadUI _saveLoadUI;

    public static SaveState I { get; private set; }

    private GameManager _gameManager;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
        _saveLoadUI.OnSelected += OnItemSelected;
        _saveLoadUI.OnBack += OnBack;
        _saveLoadUI.Show();
    }

    public override void Execute()
    {
        _saveLoadUI.HandleUpdate();
    }

    public override void Exit(bool sfx = true)
    {
        _saveLoadUI.Close();
        _saveLoadUI.OnSelected -= OnItemSelected;
        _saveLoadUI.OnBack -= OnBack;
    }

    private void OnItemSelected(int selection)
    {
        StartCoroutine(_saveLoadUI.TrySave());
    }

    private void OnBack()
    {
        _gameManager.StateMachine.Pop();
    }
}

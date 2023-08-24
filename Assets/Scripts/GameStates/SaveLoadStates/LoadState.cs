using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadState : State<GameManager>
{
    [SerializeField] private SaveLoadUI _saveLoadUI;

    public static LoadState I { get; private set; }

    private GameManager _gameManager;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
        _saveLoadUI.ResetSelection();
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
        StartCoroutine(_saveLoadUI.TryLoad());
    }

    private void OnBack()
    {
        _gameManager.StateMachine.Pop();
    }
}

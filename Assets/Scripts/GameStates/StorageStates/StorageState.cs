using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageState : State<GameManager>
{
    [SerializeField] private PokeStorageUI _storageUI;
    [SerializeField] private StoragePartyListUI _storagePartyListUI;
    [SerializeField] private StorageListUI _storageListUI;

    public Pokemon SelectedPokemon { get; private set; }
    public static StorageState I { get; private set; }
    public PokeStorageUI StorageUI { get => _storageUI; set => _storageUI = value; }

    private GameManager _gameManager;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
        StorageUI.gameObject.SetActive(true);
        _storagePartyListUI.Init();
        _storagePartyListUI.HideSelection();
        _storageListUI.Init();
        _storageListUI.HideSelection();
        PushPokeState.I.RightArrow.color = Color.grey;
        PopPokeState.I.LeftArrow.color = Color.grey;
        StorageUI.MessageText.text = "想要存放还是取出宝可梦？";
        StorageUI.OnSelected += OnPokemonSelected;
        StorageUI.OnBack += OnBack;
    }

    public override void Execute()
    {
        StorageUI.HandleUpdate();
    }

    public override void Exit(bool sfx = true)
    {
        StorageUI.ResetSelection();
        _storageListUI.ResetSelection();
        StorageUI.gameObject.SetActive(false);
        StorageUI.OnSelected -= OnPokemonSelected;
        StorageUI.OnBack -= OnBack;
    }

    private void OnPokemonSelected(int selection)
    {
        if (selection == 0)
        {
            StorageUI.MessageText.text = "想要存放哪一个宝可梦呢？";
            _gameManager.StateMachine.Push(PushPokeState.I);
        }
        else if (selection == 1)
        {
            StorageUI.MessageText.text = "想要取出哪一个宝可梦呢？";
            _gameManager.StateMachine.Push(PopPokeState.I);
        }

    }

    private void OnBack()
    {
        _gameManager.StateMachine.Pop();
    }
}

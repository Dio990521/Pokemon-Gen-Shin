using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenuState : State<GameManager>
{
    public List<ItemBase> AvailableItems { get; set; }

    [HideInInspector]
    public Vector2 CameraOffset;

    public static ShopMenuState I { get; private set; }

    private GameManager _gameManager;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
        StartCoroutine(StartMenuState());
    }

    private IEnumerator StartMenuState()
    {
        yield return DialogueManager.Instance.ShowDialogueText("��ӭ���٣�\n����Ҫ��ʲô��", autoClose: false);
        ChoiceState.I.Choices = new List<string>() { "������", "��������" };
        yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

        int selectedChoice = ChoiceState.I.Selection;

        if (selectedChoice == 0)
        {
            // Buy
            ShopBuyingState.I.AvailableItems = AvailableItems;
            yield return _gameManager.StateMachine.PushAndWait(ShopBuyingState.I);
        }
        else if (selectedChoice == 1)
        {
            // Sell
            yield return _gameManager.StateMachine.PushAndWait(ShopSellingState.I);
        }
        else if (selectedChoice == -1)
        {
            // Quit
            
        }

        _gameManager.StateMachine.Pop();
    }
}

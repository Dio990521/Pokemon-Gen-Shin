using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenuState : State<GameManager>
{
    public List<ItemBase> AvailableItems { get; set; }


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
        int selectedChoice = 0;
        yield return DialogueManager.Instance.ShowDialogueText("ÄãÒª¸ÉÉ¶£¿",
            waitForInput: false,
            choices: new List<string>() { "Âò", "Âô" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

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

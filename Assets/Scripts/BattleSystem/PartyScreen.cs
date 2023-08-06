using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] private Text messageText;
    [SerializeField] private PartyMemberUI[] memberSlots;
    private List<Pokemon> pokemons;
    private PokemonParty party;

    private int selection = 0;
    private int prevSelection = -1;

    public Pokemon SelectedMember => pokemons[selection];

    /// <summary>
    /// Party screen can be called from different states like ActionSelection, RuuningTurn, AboutToUse
    /// </summary>
    public BattleState? CalledFrom { get; set; }

    public void Init()
    {
        party = PokemonParty.GetPlayerParty();
        SetPartyData();

        party.OnUpdated += SetPartyData;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        selection = 0;
        prevSelection = -1;
    }

    public void SetPartyData()
    {
        pokemons = party.Pokemons;

        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < pokemons.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].Init(pokemons[i]);
                memberSlots[i].SetSLot();
            }
            else
            {
                memberSlots[i].SetEmptySlot();
            }
        }

        UpdateMemberSelection(selection);

        SetMessageText("选择一个宝可梦。");
    }

    public void HandleUpdate(Action onSelected, Action onBack)
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selection += 1;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selection -= 1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selection += 1;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selection -= 1;
        }

        selection = Mathf.Clamp(selection, 0, pokemons.Count - 1);

        if (selection != prevSelection)
        {
            UpdateMemberSelection(selection);
        }
        prevSelection = selection;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            AudioManager.Instance.PlaySE(SFX.CONFIRM);
            onSelected?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            AudioManager.Instance.PlaySE(SFX.CANCEL);
            onBack?.Invoke();
        }
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < pokemons.Count; i++)
        {
            if (i == selectedMember)
            {
                memberSlots[i].SetSelected(true);
            }
            else
            {
                memberSlots[i].SetSelected(false);
            }
            
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}

using PokeGenshinUtils.SelectionUI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class PartyScreen : SelectionUI<PartyMemberUI>
{
    [SerializeField] private Text messageText;
    [SerializeField] private PartyMemberUI[] memberSlots;
    [SerializeField] private GameObject _buttonBG;
    [SerializeField] private Image _cancelBall;
    [SerializeField] private Sprite _cancelBallSprite1;
    [SerializeField] private Sprite _cancelBallSprite2;
    private List<Pokemon> pokemons;
    private PokemonParty party;

    public Pokemon SelectedMember => pokemons[selectedItem];

    public List<Pokemon> Pokemons { get => pokemons; set => pokemons = value; }

    public int SelectedItem => selectedItem;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
        SetSelectionSettings(SelectionType.List, 1);

        party = PokemonParty.GetPlayerParty();
        SetPartyData();

        party.OnUpdated += SetPartyData;
    }

    public void Show()
    {
        gameObject.SetActive(true);
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

        SetItems(memberSlots.Take(pokemons.Count).ToList());

        SetMessageText("选择一个宝可梦。");
    }

    public void SwitchPokemonSlot(int index1, int index2)
    {
        (pokemons[index2], pokemons[index1]) = (pokemons[index1], pokemons[index2]);
        SetPartyData();
    }

    public override void ClampSelection()
    {
        selectedItem = Mathf.Clamp(selectedItem, 0, _items.Count);
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        if (selectedMember == pokemons.Count)
        {
            _buttonBG.SetActive(true);
            _cancelBall.sprite = _cancelBallSprite2;
        }
        else
        {
            _buttonBG.SetActive(false);
            _cancelBall.sprite = _cancelBallSprite1;
        }

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

    public override void UpdateSelectionUI()
    {
        if (selectedItem == pokemons.Count)
        {
            _buttonBG.SetActive(true);
            _cancelBall.sprite = _cancelBallSprite2;
        }
        else
        {
            _buttonBG.SetActive(false);
            _cancelBall.sprite = _cancelBallSprite1;
        }
        base.UpdateSelectionUI();
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }

}

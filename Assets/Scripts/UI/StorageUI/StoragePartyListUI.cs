using PokeGenshinUtils.SelectionUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoragePartyListUI : SelectionUI<ItemSlotUI>
{
    [SerializeField] private ItemSlotUI _itemSlotUI;
    [SerializeField] private GameObject _itemList;

    private List<ItemSlotUI> _slotUIList;
    private List<Pokemon> _pokemons;

    public List<Pokemon> Pokemons { get => _pokemons; set => _pokemons = value; }

    private void Start()
    {
        Storage.GetStorage().OnPartyUpdate += UpdateItemList;
    }

    public void Init()
    {
        selectedItem = -1;
        Pokemons = GameManager.Instance.PartyScreen.Pokemons;
        UpdateItemList();
    }

    private void UpdateItemList()
    {
        // Clear all the existing items
        foreach (Transform child in _itemList.transform)
        {
            Destroy(child.gameObject);
        }

        _slotUIList = new List<ItemSlotUI>();

        foreach (var pokemon in Pokemons)
        {
            var slotUIObj = Instantiate(_itemSlotUI, _itemList.transform);
            slotUIObj.SetPokeData(pokemon);
            _slotUIList.Add(slotUIObj);
        }

        SetItems(_slotUIList);
    }

    public void HideSelection()
    {
        selectedItem = -1;
        prevSelection = -1;
        UpdateSelectionUI();
    }



}

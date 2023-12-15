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
    [SerializeField] private List<Sprite> elementIcons;

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
            slotUIObj.SetPokeData(pokemon, TypeToElementImage(pokemon.PokemonBase.Type1));
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

    private Sprite TypeToElementImage(PokemonType pokemonType)
    {
        switch (pokemonType)
        {
            case PokemonType.Ë®:
                return elementIcons[1];
            case PokemonType.»ð:
                return elementIcons[2];
            case PokemonType.²Ý:
                return elementIcons[3];
            case PokemonType.±ù:
                return elementIcons[4];
            case PokemonType.À×:
                return elementIcons[5];
            case PokemonType.ÑÒ:
                return elementIcons[6];
            case PokemonType.·ç:
                return elementIcons[7];
        }
        return elementIcons[0];
    }

}

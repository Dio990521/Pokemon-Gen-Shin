using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Storage : MonoBehaviour, ISavable
{
    [SerializeField] private List<Pokemon> _normalSlots; // ÆÕÍ¨
    [SerializeField] private List<Pokemon> _hydroSlots; // Ë®
    [SerializeField] private List<Pokemon> _pyroSlots;  // »ð
    [SerializeField] private List<Pokemon> _dendroSlots;  // ²Ý
    [SerializeField] private List<Pokemon> _cryoSlots;  // ±ù
    [SerializeField] private List<Pokemon> _electroSlots; //  À×
    [SerializeField] private List<Pokemon> _geoSlots;  // ÑÒ
    [SerializeField] private List<Pokemon> _anemoSlots;  // ·ç

    private List<List<Pokemon>> _allSlots;

    public event Action OnPartyUpdate;
    public event Action OnStorageUpdate;

    private PokemonParty _party;

    public static List<string> ElementCategories { get; set; } = new List<string>()
    {
        "ÆÕÍ¨", "Ë®", "»ð", "²Ý", "±ù", "À×", "ÑÒ", "·ç"
    };

    public void Init()
    {
        InitAllSlots();
    }

    private void InitAllSlots()
    {
        _allSlots = new List<List<Pokemon>>() { _normalSlots, _hydroSlots, _pyroSlots, _dendroSlots, _cryoSlots, _electroSlots, _geoSlots, _anemoSlots };
    }

    private void Start()
    {
        _party = PokemonParty.GetPlayerParty();
    }

    public static Storage GetStorage()
    {
        return GameManager.Instance.Storage;
    }

    public List<Pokemon> GetSlotsByCategory(int categoryIndex)
    {
        return _allSlots[categoryIndex];
    }

    public Pokemon GetPokemon(int pokemonIndex, int categoryIndex)
    {
        var currentSlots = GetSlotsByCategory(categoryIndex);
        if (currentSlots.Count > 0)
        {
            return currentSlots[pokemonIndex];
        }
        return null;
    }

    public void PushPokemon(Pokemon pokemon)
    {
        _party.Pokemons.Remove(pokemon);
        int category = (int) pokemon.PokemonBase.Type1;
        var currentSlots = GetSlotsByCategory(category);
        currentSlots.Add(pokemon);
        OnPartyUpdate?.Invoke();
        OnStorageUpdate?.Invoke();
    }

    public void PopPokemon(Pokemon pokemon)
    {
        int selectedCategory = (int)pokemon.PokemonBase.Type1;
        var currentSlots = GetSlotsByCategory(selectedCategory);
        currentSlots.Remove(pokemon);
        _party.AddPokemonToParty(pokemon);
        OnPartyUpdate?.Invoke();
        OnStorageUpdate?.Invoke();
    }

    public object CaptureState()
    {
        var saveData = new List<List<PokemonSaveData>>();
        foreach (var category in _allSlots)
        {
            saveData.Add(category.Select(p => p.GetSaveData()).ToList());
        }
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as List<List<PokemonSaveData>>;
        for (int i = 0;  i < saveData.Count; i++)
        {
            _allSlots[i] = saveData[i].Select(s => new Pokemon(s)).ToList();
        }

        _normalSlots = saveData[0].Select(s => new Pokemon(s)).ToList();
        _hydroSlots = saveData[1].Select(s => new Pokemon(s)).ToList();
        _pyroSlots = saveData[2].Select(s => new Pokemon(s)).ToList();
        _dendroSlots = saveData[3].Select(s => new Pokemon(s)).ToList();
        _cryoSlots = saveData[4].Select(s => new Pokemon(s)).ToList();
        _electroSlots = saveData[5].Select(s => new Pokemon(s)).ToList();
        _geoSlots = saveData[6].Select(s => new Pokemon(s)).ToList();
        _anemoSlots = saveData[7].Select(s => new Pokemon(s)).ToList();
        InitAllSlots();
        OnPartyUpdate?.Invoke();
    }
}

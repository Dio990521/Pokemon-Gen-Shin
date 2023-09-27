using DG.Tweening;
using PokeGenshinUtils.SelectionUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PokemonSelectionUI : SelectionUI<PokeballUI>
{
    [SerializeField] private List<Pokemon> _selectablePokemons;
    [SerializeField] private GameObject _display;
    [SerializeField] private Text _message;
    private Vector3 _endPos;

    public GameObject Display { get => _display; set => _display = value; }
    public Text Message { get => _message; set => _message = value; }
    public Vector3 EndPos { get => _endPos; set => _endPos = value; }
    public List<Pokemon> SelectablePokemons { get => _selectablePokemons; set => _selectablePokemons = value; }

    public Pokemon SelectedPokemon => _selectablePokemons[selectedItem];

    private void Start()
    {
        SetItems(GetComponentsInChildren<PokeballUI>().ToList());
    }

    public void Show()
    {
        EndPos = Display.transform.position;
        gameObject.SetActive(true);
        Message.text = "小田卷博士遇到麻烦了！\n请选择一个宝可梦帮助他吧！";
    }

    public PokeballUI GetSelectedPokeballUI()
    {
        return _items[selectedItem];
    }

}

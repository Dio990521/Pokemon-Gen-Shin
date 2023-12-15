using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, ISelectableItem
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text countText;
    [SerializeField] private Image _typeImage;
    public Image _cursor;

    public RectTransform rectTransform;

    public float Height => rectTransform.rect.height;

    private void Awake()
    {

    }

    public void SetData(ItemSlot itemSlot)
    {
        rectTransform = GetComponent<RectTransform>();
        nameText.text = itemSlot.Item.ItemName;
        countText.text = $"x{itemSlot.Count}";
    }

    public void SetPokeData(Pokemon pokemon, Sprite typeImage)
    {
        _typeImage.sprite = typeImage;
        rectTransform = GetComponent<RectTransform>();
        nameText.text = pokemon.PokemonBase.PokemonName;
        countText.text = $"Lv.{pokemon.Level}";
    }

    public void SetNameAndPrice(ItemBase item)
    {
        rectTransform = GetComponent<RectTransform>();
        nameText.text = item.ItemName;
        countText.text = item.Price == 0 ? $"{item.YuanshiPrice}Ô­Ê¯" : $"{item.Price}£¤";
    }

    public void Init()
    {

    }

    public void OnSelectionChanged(bool selected)
    {
        _cursor.enabled = selected;
    }
}

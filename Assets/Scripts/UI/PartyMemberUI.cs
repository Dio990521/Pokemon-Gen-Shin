using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour, ISelectableItem
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private HpBar hpBar;
    //[SerializeField] private Text expText;

    [SerializeField] private Image ballIcon;

    [SerializeField] private Sprite emptySlotImage;
    [SerializeField] private Sprite selectedSlotImage;
    [SerializeField] private Sprite notSelectedSlotImage;
    [SerializeField] private Sprite selectedBallImage;
    [SerializeField] private Sprite notSelectedBallImage;

    [SerializeField] private Text statusText;
    [SerializeField] private Image statusBg;
    [SerializeField] private Text _elementStatusText;
    [SerializeField] private Image _elementStatusBg;

    private Pokemon battlePokemon;

    // Show the essential status of the pokemon
    public void Init(Pokemon pokemon)
    {
        battlePokemon = pokemon;
        UpdateData();
        battlePokemon.OnHpChanged += UpdateData;
        battlePokemon.OnStatusChanged += UpdateData;
    }

    private void UpdateData()
    {
        nameText.text = battlePokemon.PokemonBase.PokemonName;
        levelText.text = "LV." + battlePokemon.Level;
        if (battlePokemon.Status != null)
        {
            statusText.text = battlePokemon.Status.Name;
            statusBg.color = ColorDB.StatusColors[battlePokemon.Status.Id];
            statusBg.gameObject.SetActive(true);
        }
        else
        {
            statusBg.gameObject.SetActive(false);
        }
        if (battlePokemon.ElementStatus != null)
        {
            _elementStatusText.text = battlePokemon.ElementStatus.Name;
            _elementStatusBg.color = ColorDB.StatusColors[battlePokemon.ElementStatus.Id];
            _elementStatusBg.gameObject.SetActive(true);
        }
        else
        {
            _elementStatusBg.gameObject.SetActive(false);
        }

        hpBar.SetHp((float)battlePokemon.Hp / battlePokemon.MaxHp, battlePokemon.MaxHp, battlePokemon.Hp);
    }

    public void SetEmptySlot()
    {
        GetComponent<Image>().sprite = emptySlotImage;
        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void SetSLot()
    {
        GetComponent<Image>().sprite = notSelectedSlotImage;
        for (int i = 0; i < transform.childCount - 2; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        if (battlePokemon.Status != null)
        {
            transform.GetChild(transform.childCount - 2).gameObject.SetActive(true);
        }
        if (battlePokemon.ElementStatus != null)
        {
            transform.GetChild(transform.childCount - 1).gameObject.SetActive(true);
        }
    }

    // Show the sprite when a pokemon is selected
    public void SetSelected(bool selected)
    {
        GetComponent<Image>().sprite = selected ? selectedSlotImage : notSelectedSlotImage;
        ballIcon.sprite = selected ? selectedBallImage : notSelectedBallImage;
        ballIcon.rectTransform.sizeDelta = selected ? new Vector2(ballIcon.rectTransform.sizeDelta.x, 95f) : new Vector2(ballIcon.rectTransform.sizeDelta.x, 86.33f);
        nameText.color = selected ? new Color32(79, 79, 79, 255) : new Color32(255, 255, 255, 255);
        levelText.color = selected ? new Color32(79, 79, 79, 255) : new Color32(255, 255, 255, 255);
    }

    public void Init()
    {

    }

    public void OnSelectionChanged(bool selected)
    {
        GetComponent<Image>().sprite = selected ? selectedSlotImage : notSelectedSlotImage;
        ballIcon.sprite = selected ? selectedBallImage : notSelectedBallImage;
        ballIcon.rectTransform.sizeDelta = selected ? new Vector2(ballIcon.rectTransform.sizeDelta.x, 95f) : new Vector2(ballIcon.rectTransform.sizeDelta.x, 86.33f);
        nameText.color = selected ? new Color32(79, 79, 79, 255) : new Color32(255, 255, 255, 255);
        levelText.color = selected ? new Color32(79, 79, 79, 255) : new Color32(255, 255, 255, 255);
    }
}

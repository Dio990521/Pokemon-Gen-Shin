using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private HpBar hpBar;
    [SerializeField] private Text statusText;
    [SerializeField] private Image statusBg;
    [SerializeField] private GameObject expBar;

    private Pokemon battlePokemon;

    // show the essential status of the pokemon
    public void SetData(Pokemon pokemon)
    {
        battlePokemon = pokemon;
        nameText.text = battlePokemon.PokemonBase.PokemonName;
        levelText.text = "LV." + battlePokemon.Level;
        hpBar.SetHp((float)battlePokemon.Hp / battlePokemon.MaxHp, battlePokemon.MaxHp, battlePokemon.Hp);
        SetExp();
        SetStatusText();
        battlePokemon.OnStatusChanged += SetStatusText;
    }

    void SetStatusText()
    {
        if (battlePokemon.Status == null)
        {
            statusText.text = "";
            statusBg.color = new Color32(0, 0, 0, 0);
        }
        else
        {
            statusText.text = battlePokemon.Status.Name;
            statusBg.color = ColorDB.statusColors[battlePokemon.Status.Id];
        }
    }

    public void SetExp()
    {
        if (expBar == null) return;
        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

    private float GetNormalizedExp()
    {
        int currLevelExp = battlePokemon.PokemonBase.GetExpForLevel(battlePokemon.Level);
        int nextLevelExp = battlePokemon.PokemonBase.GetExpForLevel(battlePokemon.Level + 1);

        float normalizedExp = (float)(battlePokemon.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }

    public IEnumerator SetExpSmooth()
    {
        if (expBar == null) yield break;
        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }

    // update Hp bar when the pokemon get hurt
    public IEnumerator UpdateHp()
    {
        if (battlePokemon.HpChanged)
        {
            yield return hpBar.SetHpSmooth((float)battlePokemon.Hp / battlePokemon.MaxHp, battlePokemon.Hp);
            battlePokemon.HpChanged = false;
        }
    }

}

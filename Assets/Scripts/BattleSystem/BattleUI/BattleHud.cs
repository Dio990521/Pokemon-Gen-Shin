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
    [SerializeField] private Text elementStatusText;
    [SerializeField] private Text statusText;
    [SerializeField] private Image elementStatusBG;
    [SerializeField] private Image statusBG;
    [SerializeField] private GameObject expBar;

    private Pokemon battlePokemon;

    // show the essential status of the pokemon
    public void SetData(Pokemon pokemon)
    {
        ClearData();

        battlePokemon = pokemon;
        nameText.text = battlePokemon.PokemonBase.PokemonName;
        SetLevel();
        SetExp();
        UpdateStatus();
        battlePokemon.OnStatusChanged += UpdateStatus;
        battlePokemon.OnHpChanged += UpdateHP;
    }

    private void UpdateStatus()
    {
        SetStatusText();
        SetElementStatusText();
    }

    private void SetElementStatusText()
    {
        if (battlePokemon.ElementStatus == null)
        {
            elementStatusText.text = "";
            elementStatusBG.color = new Color32(0, 0, 0, 0);
        }
        else
        {
            elementStatusText.text = battlePokemon.ElementStatus.Name;
            elementStatusBG.color = ColorDB.statusColors[battlePokemon.ElementStatus.Id];
        }
    }

    private void SetStatusText()
    {
        //print(battlePokemon.Status);
        if (battlePokemon.Status == null)
        {
            statusText.text = "";
            statusBG.color = new Color32(0, 0, 0, 0);
        }
        else
        {
            statusText.text = battlePokemon.Status.Name;
            statusBG.color = ColorDB.statusColors[battlePokemon.Status.Id];
        }
    }

    public void SetExp()
    {
        if (expBar == null) return;
        float normalizedExp = battlePokemon.GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

    public void SetLevel()
    {
        levelText.text = "LV." + battlePokemon.Level;
        hpBar.SetHp((float)battlePokemon.Hp / battlePokemon.MaxHp, battlePokemon.MaxHp, battlePokemon.Hp);
    }

    public IEnumerator SetExpSmooth(bool reset=false)
    {
        if (expBar == null) yield break;

        if (reset)
        {
            expBar.transform.localScale = new Vector3(0, 1, 1);
        }

        float normalizedExp = battlePokemon.GetNormalizedExp();
        AudioManager.Instance.PlaySE(SFX.EXP_UP);
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
        AudioManager.Instance.StopSE();
    }

    public void UpdateHP()
    {
        StartCoroutine(UpdateHpAsync(1.5f));
    }

    // update Hp bar when the pokemon get hurt
    public IEnumerator UpdateHpAsync(float duration)
    {
        yield return hpBar.SetHpSmooth((float)battlePokemon.Hp / battlePokemon.MaxHp, duration);
        yield return hpBar.StartCountdownAnimation(battlePokemon.Hp, duration);
    }

    public IEnumerator WaitForHPUpdate()
    {
        yield return new WaitUntil(() => hpBar.IsUpdating == false);
    }

    public void ClearData()
    {
        if (battlePokemon != null)
        {
            battlePokemon.OnStatusChanged -= UpdateStatus;
            battlePokemon.OnHpChanged -= UpdateHP;
        }
    }

}

using DG.Tweening;
using System;
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
    public GameObject Buffs;

    public List<Sprite> BuffIcons;
    public List<Sprite> DebuffIcons;

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
        SetBuff(battlePokemon);
        battlePokemon.OnStatusChanged += UpdateStatus;
        battlePokemon.OnHpChanged += UpdateHP;
        battlePokemon.OnBuffChanged += UpdateBuff;
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

    private void SetBuff(Pokemon pokemon)
    {
        foreach (var boosts in pokemon.StatBoosts)
        {
            if (boosts.Key == Stat.ÃüÖÐÂÊ || boosts.Key == Stat.ÉÁ±ÜÂÊ) continue;
            UpdateBuff(boosts.Key, boosts.Value);
        }
    }

    private void UpdateBuff(Stat stat, int boost)
    {
        int index = (int)stat;
        var buff = Buffs.transform.GetChild(index).GetComponent<Image>();
        if (boost == 0)
        {
            buff.gameObject.SetActive(false);
            return;
        }
        if (boost > 0)
        {
            buff.sprite = BuffIcons[(boost - 1) * 5 + index];
        }
        else
        {
            buff.sprite = DebuffIcons[(-boost - 1) * 5 + index];
        }
        buff.gameObject.SetActive(true);
    }

    public void ClearBuffs()
    {
        for (int i = 0; i < Buffs.transform.childCount; i++)
        {
            Transform childTransform = Buffs.transform.GetChild(i);
            childTransform.gameObject.SetActive(false);
        }
    }

    private void SetStatusText()
    {
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
        if (expBar == null || battlePokemon.Level == 100)
        {
            yield break;
        }

        if (reset)
        {
            expBar.transform.localScale = new Vector3(0, 1, 1);
        }

        float normalizedExp = battlePokemon.GetNormalizedExp();
        AudioManager.Instance.PlaySE(SFX.EXP_UP);
        yield return expBar.transform.DOScaleX(normalizedExp, (normalizedExp - expBar.transform.localScale.x) * 2f).WaitForCompletion();
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

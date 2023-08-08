using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AchievementUI : MonoBehaviour
{
    [SerializeField] private Text _money;
    [SerializeField] private Text _pokeDex;
    [SerializeField] private Text _playtime;
    [SerializeField] private List<Text> _pokemonCollections;
    [SerializeField] private List<Image> _badgeImages;
    [SerializeField] private List<KeyItem> _badges;

    public void Show()
    {
        _money.text = Wallet.i.Money.ToString();
        _pokeDex.text = $"{AchievementManager.Instance.GetTotalProgress().ToString("F1")}%";
        _playtime.text = GameManager.Instance.GamePlayTime;
        foreach (Achievement tag in Enum.GetValues(typeof(Achievement)))
        {
            int index = (int)tag;
            if (index % 2 == 0)
            {
                _pokemonCollections[index].text = $"{AchievementManager.Instance.GetProgress(tag)}/{AchievementManager.Instance.PokemonCount[(int)tag]}";
            }
            else
            {
                _pokemonCollections[index].text = $"(ฝ๘ปฏ {AchievementManager.Instance.GetProgress(tag)}/{AchievementManager.Instance.PokemonCount[(int)tag]})";
            }
        }
        var inventory = Inventory.GetInventory();
        for (int i = 0; i < _badgeImages.Count; i++)
        {
            if (inventory.HasItem(_badges[i]))
            {
                _badgeImages[i].enabled = true;
            }
            else
            {
                _badgeImages[i].enabled = false;
            }
        }
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        GameManager.Instance.StartFreeRoamState();
    }

    public void HandleUpdate()
    {

        if (Input.GetKeyDown(KeyCode.X))
        {
            AudioManager.Instance.PlaySE(SFX.CANCEL);
            Close();
        }
    }
}

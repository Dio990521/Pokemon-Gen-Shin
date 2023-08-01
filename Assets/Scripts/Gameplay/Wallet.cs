using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : MonoBehaviour, ISavable
{
    [SerializeField] private int money;
    [SerializeField] private ItemBase yuanshi;

    public event Action OnMoneyChanged;

    public static Wallet i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    public int Money => money;

    public ItemBase Yuanshi { get {  return yuanshi; } }

    public void AddMoney(int amount, bool playSE=true)
    {
        if (playSE)
        {
            AudioManager.Instance.PlaySE(SFX.OBTAIN_BERRY, true);
        }
        money += amount;
        OnMoneyChanged?.Invoke();
    }

    public void TakeMoney(int amount)
    {
        money -= amount;
        OnMoneyChanged?.Invoke();
    }

    public bool HasMoney(int amount)
    {
        return amount <= money;
    }

    public object CaptureState()
    {
        return money;
    }

    public void RestoreState(object state)
    {
        money = (int)state;
    }
}

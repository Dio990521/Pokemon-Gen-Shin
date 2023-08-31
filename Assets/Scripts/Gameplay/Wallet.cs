using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : MonoBehaviour, ISavable
{
    [SerializeField] private int money;
    [SerializeField] private ItemBase yuanshi;

    private int _visaLimit;
    public event Action OnMoneyChanged;

    public static Wallet I { get; private set; }

    private void Awake()
    {
        I = this;
        _visaLimit = 100000;
    }

    public int Money => money;

    public ItemBase Yuanshi { get {  return yuanshi; } }

    public int VisaLimit { get => _visaLimit; set => _visaLimit = value; }

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

    public void IncreaseVisaLimit(int amount)
    {
        _visaLimit += amount;
    }

    public void DecreaseVisaLimit(int amount)
    {
        _visaLimit -= amount;
    }

    public object CaptureState()
    {
        var saveData = new WalletSaveData
        {
            money = money,
            visaLimit = _visaLimit
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (WalletSaveData)state;
        money = saveData.money;
        _visaLimit = saveData.visaLimit;
    }
}

[Serializable]
public class WalletSaveData
{
    public int money;
    public int visaLimit;
}

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

    public bool IsUnlimited;

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
            AudioManager.Instance.PlaySE(SFX.MONEY, true);
        }
        money += amount;
        OnMoneyChanged?.Invoke();
    }

    public void TakeMoney(int amount)
    {
        money -= amount;
        OnMoneyChanged?.Invoke();
    }

    public void TakeMoneyPercentage(float percentage)
    {
        money = (int)(money * (1f - percentage));
        OnMoneyChanged?.Invoke();
    }

    public bool TryTakeYuanshiPercentage(float percentage)
    {
        var inventory = Inventory.GetInventory();
        if (inventory.HasItem(yuanshi))
        {
            inventory.RemoveItem(yuanshi, (int)(inventory.GetItemCount(yuanshi) * percentage));
            OnMoneyChanged?.Invoke();
            return true;
        }
        return false;
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
            visaLimit = _visaLimit,
            IsUnlimited = IsUnlimited
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (WalletSaveData)state;
        money = saveData.money;
        _visaLimit = saveData.visaLimit;
        IsUnlimited = saveData.IsUnlimited;
    }
}

[Serializable]
public class WalletSaveData
{
    public int money;
    public int visaLimit;
    public bool IsUnlimited;
}

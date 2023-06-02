using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    [SerializeField] private int money;

    public event Action OnMoneyChanged;

    public static Wallet i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    public int Money => money;

    public void AddMoney(int amount)
    {
        money += amount;
        OnMoneyChanged?.Invoke();
    }

    public void TakeMoney(int amount)
    {
        money -= amount;
        OnMoneyChanged?.Invoke();
    }
}

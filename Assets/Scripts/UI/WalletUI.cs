using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class WalletUI : MonoBehaviour
{
    [SerializeField] private Text moneyText;
    [SerializeField] private Text yuanshiText;

    private void Start()
    {
        Wallet.i.OnMoneyChanged += SetMoneyText;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        SetMoneyText();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void SetMoneyText()
    {
        StringBuilder money = new StringBuilder();
        money.Append("持有摩拉：").Append(Wallet.i.Money).Append("￥");
        moneyText.text = money.ToString();
        money.Clear();
        money.Append("持有原石：").Append(Inventory.GetInventory().GetItemCount(Wallet.i.Yuanshi));
        yuanshiText.text = money.ToString();
    }

}

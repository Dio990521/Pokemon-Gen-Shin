using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class WalletUI : MonoBehaviour
{
    [SerializeField] private Text moneyText;

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
        money.Append("³ÖÓÐ½ðÇ®£º");
        money.Append(Wallet.i.Money);
        money.Append("£¤");
        moneyText.text = money.ToString();
    }
}

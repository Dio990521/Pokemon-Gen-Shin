using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    [SerializeField] private List<ItemBase> availableItems;
    public IEnumerator Trade()
    {
        yield return ShopController.i.StartTrading(this);
    }

    public List<ItemBase> AvailableItems => availableItems;
}

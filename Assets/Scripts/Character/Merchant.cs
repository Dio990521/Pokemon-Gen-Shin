using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    [SerializeField] private List<ItemBase> availableItems;
    public IEnumerator Trade()
    {
        ShopMenuState.I.AvailableItems = availableItems;
        yield return GameManager.Instance.StateMachine.PushAndWait(ShopMenuState.I);
    }

    public List<ItemBase> AvailableItems => availableItems;
}

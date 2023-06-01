using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{

    public IEnumerator Trade()
    {
        yield return ShopController.i.StartTrading(this);
    }
}

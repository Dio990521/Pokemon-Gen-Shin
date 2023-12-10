using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    [SerializeField] private List<ItemBase> availableItems;
    [SerializeField] private List<ItemBase> extraItems;
    [SerializeField] private Vector2 shopCameraOffset;

    public IEnumerator Trade()
    {
        ShopMenuState.I.AvailableItems = availableItems;
        ShopMenuState.I.CameraOffset = shopCameraOffset;
        if (GameKeyManager.Instance.GetBoolValue(CutsceneName.获得冰系道馆徽章.ToString()))
        {
            ShopMenuState.I.AvailableItems.AddRange(extraItems);
        }
        yield return GameManager.Instance.StateMachine.PushAndWait(ShopMenuState.I);
    }

    public List<ItemBase> AvailableItems => availableItems;

    public List<ItemBase> ExtraItems { get => extraItems; set => extraItems = value; }
}

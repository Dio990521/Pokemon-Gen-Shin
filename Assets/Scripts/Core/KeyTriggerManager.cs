using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerKey
{

}

public class KeyTriggerManager : MonoBehaviour
{
    public static Dictionary<TriggerKey, int> KeyTriggers { get; set; } = 
        new Dictionary<TriggerKey, int>() { };

    public static KeyTriggerManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }
}

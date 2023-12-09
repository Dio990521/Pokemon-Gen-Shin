using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCZ : MonoBehaviour
{

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if (GameKeyManager.Instance.GetBoolValue(CutsceneName.¼¤»îdcz.ToString()))
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

}

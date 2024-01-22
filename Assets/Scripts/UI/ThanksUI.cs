using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThanksUI : MonoBehaviour
{

    public void GoToDCZ()
    {
        Application.OpenURL("https://space.bilibili.com/1879116");
    }

    public void GoToCV1()
    {
        Application.OpenURL("https://space.bilibili.com/96644933");
    }

    public void GoToCV2()
    {
        Application.OpenURL("https://space.bilibili.com/35103308");
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}

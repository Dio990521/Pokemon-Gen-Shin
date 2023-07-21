using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCZ : MonoBehaviour, ISavable
{
    private bool _isShow;

    private void Awake()
    {
        _isShow = false;
        GameEventManager.Instance.AddEventListener("DCZ", Show);
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

    }

    private void Show()
    {
        _isShow = true;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public object CaptureState()
    {
        return _isShow;
    }

    public void RestoreState(object state)
    {
        _isShow = (bool)state;
        if (_isShow)
        {
            Show();
        }
    }

}

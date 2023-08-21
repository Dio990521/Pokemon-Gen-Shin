using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSlot : MonoBehaviour, ISelectableItem
{
    [SerializeField] private Image _cursor;

    public void Init()
    {

    }

    public void OnSelectionChanged(bool selected)
    {
        _cursor.enabled = selected;
    }

    public void SetText(string text)
    {
        GetComponentInChildren<Text>().text = text;
    }
}

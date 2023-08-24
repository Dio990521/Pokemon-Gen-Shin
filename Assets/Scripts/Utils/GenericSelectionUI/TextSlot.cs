using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSlot : MonoBehaviour, ISelectableItem
{
    [SerializeField] private Image _cursor;
    private float _textWidth;

    public void Init()
    {

    }

    public void OnSelectionChanged(bool selected)
    {
        _cursor.enabled = selected;
    }

    public void SetText(string text)
    {
        var textUI = GetComponentInChildren<Text>();
        textUI.text = text;
        _textWidth = textUI.preferredWidth;
    }

    public float GetTextWidth()
    {
        return _textWidth;
    }
}

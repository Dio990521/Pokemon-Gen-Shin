using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedSelectorUI : MonoBehaviour, ISelectableItem
{

    [SerializeField] private Image _selector;

    public void Init()
    {

    }

    public void OnSelectionChanged(bool selected)
    {
        _selector.enabled = selected;
    }


}

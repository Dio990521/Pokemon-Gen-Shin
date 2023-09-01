using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokeballUI : MonoBehaviour, ISelectableItem
{
    [SerializeField] private AnimatedSprite _pokeBall;

    public void Init()
    {

    }

    public void OnSelectionChanged(bool selected)
    {
        _pokeBall.enabled = selected;
        _pokeBall.transform.GetChild(0).gameObject.SetActive(selected);
    }

}

using PokeGenshinUtils.SelectionUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PartyMenu : SelectionUI<TextSlot>
{
    [SerializeField] private List<TextSlot> _slots;

    private void Awake()
    {
        SetItems(_slots);
    }


}

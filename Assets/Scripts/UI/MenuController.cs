using PokeGenshinUtils.SelectionUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : SelectionUI<TextSlot>
{

    private void Start()
    {
        SetItems(GetComponentsInChildren<TextSlot>().ToList());
    }

}

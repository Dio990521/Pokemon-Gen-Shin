using PokeGenshinUtils.SelectionUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionSelectionUI : SelectionUI<TextSlot>
{
    private void Start()
    {
        SetSelectionSettings(SelectionType.Grid, 2);
        SetItems(GetComponentsInChildren<TextSlot>().ToList());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectableItem
{
    public void Init();
    public void OnSelectionChanged(bool selected);
}

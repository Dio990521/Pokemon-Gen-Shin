using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceText : MonoBehaviour
{
    [SerializeField] private Text text;
    public Text TextField => text;

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            text.color = ColorDB.textColors["OnChoice"];
        }
        else
        {
            text.color = ColorDB.textColors["NotChoice"];
        }
    }

}

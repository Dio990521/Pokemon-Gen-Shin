using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardInput : MonoBehaviour
{

    public static bool ConfirmKeyDown(KeyCode confirmKeyCode)
    {
        AudioManager.instance.PlaySE(SFX.CONFIRM);
        return Input.GetKeyDown(confirmKeyCode);
    }
}

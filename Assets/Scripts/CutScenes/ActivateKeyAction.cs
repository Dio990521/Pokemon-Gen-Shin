using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateKeyAction : CutsceneAction
{
    [SerializeField] private CutsceneName _cutsceneName;

    public override IEnumerator Play()
    {
        if (_cutsceneName != CutsceneName.None)
        {
            GameKeyManager.Instance.SetBoolValue(_cutsceneName.ToString(), true);
        }
        yield return null;
    }
}

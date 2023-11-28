using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadConnectedSceneAction : CutsceneAction
{
    public override IEnumerator Play()
    {
        Cutscene.IsReloadConnectedScene = true;
        yield return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadConnectedSceneAction : CutsceneAction
{
    public override IEnumerator Play()
    {
        yield return Fader.FadeIn(1f);
        yield return GameManager.Instance.RefreshScene();
        yield return Fader.FadeOut(1f);
    }
}

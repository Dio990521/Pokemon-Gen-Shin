using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySEAction : CutsceneAction
{
    [SerializeField] private SFX SFXName;
    [SerializeField] private bool PauseMusic;

    public override IEnumerator Play()
    {
        AudioManager.Instance.PlaySE(SFXName, PauseMusic);
        yield return null;
    }
}

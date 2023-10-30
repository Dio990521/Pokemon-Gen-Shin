using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SwitchBGMAction : CutsceneAction
{
    [SerializeField] private BGM _bgm;
    [SerializeField] private float _fadeDuration;

    public override IEnumerator Play()
    {
        AudioManager.Instance.PlayMusic(_bgm, _fadeDuration);
        yield return null;
    }
}

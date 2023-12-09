using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitAction : CutsceneAction
{
    [SerializeField] private float _waitTime;

    public override IEnumerator Play()
    {
        yield return new WaitForSeconds(_waitTime);
    }
}

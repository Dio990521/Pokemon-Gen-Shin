using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObjectAction : CutsceneAction
{
    [SerializeField] private GameObject go;

    public override IEnumerator Play()
    {
        go.SetActive(false);
        yield break;
    }
}

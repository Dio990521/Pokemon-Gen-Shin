using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportObjectAction : CutsceneAction
{
    [SerializeField] private GameObject go;
    [SerializeField] private Vector2 position;

    public override IEnumerator Play()
    {
        go.transform.position = position;
        yield break;
    }
}

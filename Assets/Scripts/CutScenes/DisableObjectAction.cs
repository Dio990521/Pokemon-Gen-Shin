using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObjectAction : CutsceneAction
{
    [SerializeField] private GameObject go;

    public override IEnumerator Play()
    {
        go.SetActive(false);
        var collider = go.GetComponent<Collider2D>();
        var spriteRenderer = go.GetComponent<SpriteRenderer>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
        yield break;
    }
}

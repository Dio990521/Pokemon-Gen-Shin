using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObjectAction : CutsceneAction
{
    [SerializeField] private GameObject go;

    public override IEnumerator Play()
    {
        go.SetActive(true);
        var collider = go.GetComponent<Collider2D>();
        var spriteRenderer = go.GetComponent<SpriteRenderer>();
        if (collider != null)
        {
            collider.enabled = true;
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
        yield break;
    }

}

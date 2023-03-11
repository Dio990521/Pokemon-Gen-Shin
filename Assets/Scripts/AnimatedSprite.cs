using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedSprite : MonoBehaviour
{
    public Sprite[] sprites;
    public float frameRate = 1f / 6f;

    private SpriteRenderer spriteRenderer;
    private int frame = 0;

    public bool isLoop = true;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (isLoop)
        {
            InvokeRepeating(nameof(LoopAnimate), frameRate, frameRate);
        }
        else
        {
            StartCoroutine(Animate());
        }
        
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void LoopAnimate()
    {
        frame++;

        if (frame >= sprites.Length)
        {
            frame = 0;
        }

        if (frame >= 0 && frame < sprites.Length)
        {
            spriteRenderer.sprite = sprites[frame];
        }

    }

    private IEnumerator Animate()
    {
        while (frame >= 0 && frame < sprites.Length)
        {
            spriteRenderer.sprite = sprites[frame];
            yield return new WaitForSeconds(frameRate);
            frame++;
        }

        Destroy(gameObject);
    }

}

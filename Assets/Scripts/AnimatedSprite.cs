using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedSprite : MonoBehaviour
{
    public Sprite[] sprites;
    public float frameRate = 1f / 6f;

    private SpriteRenderer spriteRenderer;
    private Image image;
    private int _frame = 0;

    public bool isLoop = true;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            image = GetComponent<Image>();
        }
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
        _frame++;

        if (_frame >= sprites.Length)
        {
            _frame = 0;
        }

        if (_frame >= 0 && _frame < sprites.Length)
        {
            SetSprite();
        }

    }

    private IEnumerator Animate()
    {
        while (_frame >= 0 && _frame < sprites.Length)
        {
            SetSprite();
            yield return new WaitForSeconds(frameRate);
            _frame++;
        }

        Destroy(gameObject);
    }

    private void SetSprite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprites[_frame];
        }
        else
        {
            image.sprite = sprites[_frame];
        }
    }

}

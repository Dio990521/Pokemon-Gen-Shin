using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator
{
    private SpriteRenderer spriteRenderer;
    private List<Sprite> frames;
    private float frameRate;

    private int currentFrame;
    private float timer;

    public List<Sprite> Frames { get { return frames; } }

    public SpriteAnimator(SpriteRenderer spriteRenderer, List<Sprite> frames, float frameRate=0.16f)
    {
        this.spriteRenderer = spriteRenderer;
        this.frames = frames;
        this.frameRate = frameRate;
    }

    public void Start()
    {
        currentFrame = 1;
        timer = 0;
        spriteRenderer.sprite = frames[1];
    }

    public void HandleUpdate()
    {
        timer += Time.deltaTime;
        if (timer > frameRate)
        {
            currentFrame = (currentFrame + 1) % frames.Count;
            spriteRenderer.sprite = frames[currentFrame];
            timer -= frameRate;
        }
    }
}

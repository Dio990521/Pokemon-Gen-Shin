using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    // Parameters
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }
    public bool IsJumping { get; set; }
    public bool IsRunning { get; set; }

    public bool IsSurfing { get; set; }

    [SerializeField] private List<Sprite> walkDownSprites;
    [SerializeField] private List<Sprite> walkUpSprites;
    [SerializeField] private List<Sprite> walkLeftSprites;
    [SerializeField] private List<Sprite> walkRightSprites;
    [SerializeField] private FacingDirection defaultDirection = FacingDirection.Down;

    [SerializeField] private List<Sprite> runDownSprites;
    [SerializeField] private List<Sprite> runUpSprites;
    [SerializeField] private List<Sprite> runLeftSprites;
    [SerializeField] private List<Sprite> runRightSprites;

    // States
    private SpriteAnimator walkDownAnim;
    private SpriteAnimator walkUpAnim;
    private SpriteAnimator walkLeftAnim;
    private SpriteAnimator walkRightAnim;

    private SpriteAnimator runDownAnim;
    private SpriteAnimator runUpAnim;
    private SpriteAnimator runLeftAnim;
    private SpriteAnimator runRightAnim;

    private SpriteAnimator currentAnim;

    // References
    private SpriteRenderer spriteRenderer;

    private bool wasPreviouslyMoving;

    public FacingDirection DefaultDirection { get { return defaultDirection; } }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(spriteRenderer, walkDownSprites);
        walkUpAnim = new SpriteAnimator(spriteRenderer, walkUpSprites);
        walkLeftAnim = new SpriteAnimator(spriteRenderer, walkLeftSprites);
        walkRightAnim = new SpriteAnimator(spriteRenderer, walkRightSprites);
        runDownAnim = new SpriteAnimator(spriteRenderer, runDownSprites, 0.12f);
        runUpAnim = new SpriteAnimator(spriteRenderer, runUpSprites, 0.12f);
        runLeftAnim = new SpriteAnimator(spriteRenderer, runLeftSprites, 0.12f);
        runRightAnim = new SpriteAnimator(spriteRenderer, runRightSprites, 0.12f);
        SetFacingDirection(defaultDirection);

        currentAnim = walkDownAnim;
    }

    private void Update()
    {
        var prevAnim = currentAnim;
        if (!IsRunning)
        {
            if (MoveX == 1)
            {
                currentAnim = walkRightAnim;
            }
            else if (MoveX == -1)
            {
                currentAnim = walkLeftAnim;
            }
            else if (MoveY == 1)
            {
                currentAnim = walkUpAnim;
            }
            else if (MoveY == -1)
            {
                currentAnim = walkDownAnim;
            }
        }
        else if (IsRunning && IsMoving)
        {
            if (MoveX == 1)
            {
                currentAnim = runRightAnim;
            }
            else if (MoveX == -1)
            {
                currentAnim = runLeftAnim;
            }
            else if (MoveY == 1)
            {
                currentAnim = runUpAnim;
            }
            else if (MoveY == -1)
            {
                currentAnim = runDownAnim;
            }
        }


        if (currentAnim != prevAnim || IsMoving != wasPreviouslyMoving)
        {
            currentAnim.Start();
        }
        if (IsJumping)
        {
            spriteRenderer.sprite = currentAnim.Frames[0];
        }
        else if (IsMoving)
        {
            currentAnim.HandleUpdate();
        }
        else
        {
            spriteRenderer.sprite = currentAnim.Frames[1];
        }
        wasPreviouslyMoving = IsMoving;


    }

    public void SetFacingDirection(FacingDirection dir)
    {
        MoveX = 0;
        MoveY = 0;
        if (dir == FacingDirection.Right)
        {
            MoveX = 1;
        }
        else if (dir == FacingDirection.Left)
        {
            MoveX = -1;
        }
        else if (dir == FacingDirection.Up)
        {
            MoveY = 1;
        }
        else if (dir == FacingDirection.Down)
        {
            MoveY = -1;
        }
        else if (dir == FacingDirection.ToPlayer)
        {
            var player = GameManager.Instance.PlayerController;
            MoveX = -player.Character.Animator.MoveX;
            MoveY = -player.Character.Animator.MoveY;
        }
    }

    public void SetFacingDirectionToTarget(Character target)
    {
        var dir = (target.transform.position - transform.position).normalized;
        int dirX = (int)dir.x;
        int dirY = (int)dir.y;

        if (dirX > 0 && dirY == 0)
        {
            MoveX = 1;
        }
        else if (dirX < 0 && dirY == 0)
        {
            MoveX = -1;
        }
        else if (dirX == 0 && dirY > 0)
        {
            MoveY = 1;
        }
        else if (dirX == 0 && dirY < 0)
        {
            MoveY = -1;
        }

    }

}

public enum FacingDirection { Left, Right, Up, Down, None, ToPlayer }

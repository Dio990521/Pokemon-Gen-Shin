using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Character : MonoBehaviour
{
    public float moveSpeed;
    private CharacterAnimator animator;

    public bool IsMoving {  get; set; }

    public float OffsetY { get; private set; } = 0.15f;

    public CharacterAnimator Animator
    {
        get => animator;
    }

    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
        SetPositionAndSnapToTile(transform.position);
    }

    public FacingDirection GetCharacterDirection()
    {
        float moveX = animator.MoveX;
        float moveY = animator.MoveY;

        if (moveX == 1 && moveY == 0)
        {
            return FacingDirection.Right;
        }
        else if (moveX == -1 && moveY == 0)
        {
            return FacingDirection.Left;
        }
        else if (moveX == 0 && moveY == 1)
        {
            return FacingDirection.Up;
        }

        return FacingDirection.Down;

    }

    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f + OffsetY;
        transform.position = pos;
    }

    public IEnumerator Move(Vector2 moveVec, Action OnMoveOver = null, bool checkCollisions=true)
    {
       
        animator.MoveX = Mathf.Clamp(moveVec.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVec.y, -1f, 1f);
        Vector3 targetPos = transform.position;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;

        Ledge ledge = CheckForLedge(targetPos);
        if (ledge != null)
        {
            if (ledge.TryToJump(this, moveVec))
            {
                yield break;
            }
        }

        if (!IsPathClear(targetPos) && checkCollisions)
        {
            yield break; 
        }

        if (animator.IsSurfing && Physics2D.OverlapCircle(targetPos, 0.3f, GameLayers.instance.WaterLayer) == null)
        {
            animator.IsSurfing = false;
        }

        IsMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveSpeed);
            yield return null;
        }
        transform.position = targetPos;
        IsMoving = false;

        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    private bool IsPathClear(Vector3 targetPos)
    {
        var diff = targetPos - transform.position;
        var direction = diff.normalized;
        var collisionLayer = GameLayers.instance.ObjectMask | GameLayers.instance.InteractableLayer | GameLayers.instance.PlayerLayer;
        if (!animator.IsSurfing)
        {
            collisionLayer |= GameLayers.instance.WaterLayer;
        }
        if (Physics2D.BoxCast(transform.position + direction, new Vector2(0.2f, 0.2f), 0f, direction, diff.magnitude - 1, 
            collisionLayer))
        {
            return false;
        }
        return true;
    }

    // Check collision on the map
    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.15f, GameLayers.instance.ObjectMask 
            | GameLayers.instance.InteractableLayer) != null)
        {
            return false;
        }
        return true;
    }

    private Ledge CheckForLedge(Vector3 targetPos)
    {
        var collider = Physics2D.OverlapCircle(targetPos, 0.15f, GameLayers.instance.LedgeLayer);
        return collider?.GetComponent<Ledge>();
    }

    private void CheckForDoor(Vector3 targetPos)
    {
        var collider = Physics2D.OverlapCircle(targetPos, 0.15f, GameLayers.instance.DoorLayer);
    }

    public void LookTowards(Vector3 targetPos)
    {
        var xDiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var yDiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);
        if (xDiff == 0 || yDiff == 0)
        {
            animator.MoveX = Mathf.Clamp(xDiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(yDiff, -1f, 1f); 
        }
        else
        {
            Debug.LogError("Error in LookTowards: you can't ask the character to look diagonally");
        }

    }
}

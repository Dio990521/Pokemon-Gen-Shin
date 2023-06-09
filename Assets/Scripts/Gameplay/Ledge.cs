using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ledge : MonoBehaviour
{
    [SerializeField] private int xDir;
    [SerializeField] private int yDir;

    public bool TryToJump(Character character, Vector2 moveDir)
    {
        if (moveDir.x == xDir && moveDir.y == yDir)
        {
            AudioManager.instance.PlaySE(SFX.JUMP);
            StartCoroutine(Jump(character));
            return true;
        }
        return false;
    }

    private IEnumerator Jump(Character character)
    {
        GameManager.Instance.PauseGame(true);
        character.Animator.IsJumping = true;
        var jumpDest = character.transform.position + new Vector3(xDir, yDir) * 2;
        yield return character.transform.DOJump(jumpDest, 1f, 1, 0.6f).WaitForCompletion();
        character.Animator.IsJumping = false;
        GameManager.Instance.PauseGame(false);
    }
}

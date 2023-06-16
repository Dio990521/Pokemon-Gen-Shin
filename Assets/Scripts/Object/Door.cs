using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private RuntimeAnimatorController controller;
    private bool opened = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = controller;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!opened)
        {
            if (collision.CompareTag("Player"))
            {
                var player = collision.GetComponent<PlayerController>();
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    StartCoroutine(PlayDoorAnimation(player));
                }
            }
        }
    }

    public IEnumerator PlayDoorAnimation(PlayerController player)
    {
        player.Character.IsMoving = false;
        player.Character.Animator.SetFacingDirection(FacingDirection.Up);
        AudioManager.instance.PlaySE(SFX.OPEN_DOOR);
        GameManager.Instance.PauseGame(true);
        opened = true;
        animator.SetBool("isOpen", true);
        yield return new WaitForSeconds(0.5f);
        yield return player.Character.Move(new Vector2(0, 1), player.OnMoveOver, false);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isOpen", false);
        opened = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private RuntimeAnimatorController controller;
    private bool opened = false;
    [SerializeField] private bool isAutoDoor = false;

    [SerializeField] private ItemBase _key;
    private bool _isUnlock;
    private bool _isDialogue;

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
                StartCoroutine(CheckDoor(collision));
            }
        }
    }

    private IEnumerator CheckDoor(Collider2D collision)
    {
        var player = collision.GetComponent<PlayerController>();
        if (!player.Character.IsMoving && GameManager.Instance.StateMachine.CurrentState == FreeRoamState.I && 
            Input.GetKey(KeyCode.UpArrow))
        {
            if (_key != null && !Inventory.GetInventory().HasItem(_key))
            {
                if (!_isDialogue)
                {
                    AudioManager.Instance.PlaySE(SFX.LOCK);
                    _isDialogue = true;
                    yield return DialogueManager.Instance.ShowDialogueText("√≈…œÀ¯¡À°£");
                    _isDialogue = false;
                }
                yield break;
            }
            else if (_key != null && Inventory.GetInventory().HasItem(_key) && !_isUnlock)
            {
                _isUnlock = true;
            }
            GameManager.Instance.PauseGame(true);
            yield return PlayDoorAnimation(player);
        }
    }

    public IEnumerator PlayDoorAnimation(PlayerController player)
    {
        player.Character.Animator.SetFacingDirection(FacingDirection.Up);
        if (isAutoDoor)
        {
            AudioManager.Instance.PlaySE(SFX.HEALTH_CENTER_IN);
        }
        else
        {
            AudioManager.Instance.PlaySE(SFX.OPEN_DOOR);
        }
        opened = true;
        animator.SetBool("isOpen", true);
        player.Character.Animator.IsMoving = false;
        yield return new WaitForSeconds(0.5f);
        player.Character.Animator.IsMoving = true;
        yield return player.Character.Move(new Vector2(0, 1), player.OnMoveOver, false);
        yield return new WaitForSeconds(0.5f);
        player.Character.Animator.IsMoving = false;
        animator.SetBool("isOpen", false);
        opened = false;
    }
}

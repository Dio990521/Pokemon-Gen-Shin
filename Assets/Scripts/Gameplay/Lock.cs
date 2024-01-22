using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour, InteractableObject
{
    [SerializeField] private ItemBase _keyItem;
    [SerializeField] private int _total;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;

    public PuzzleName PuzzleName;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _boxCollider  = GetComponentInChildren<BoxCollider2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (GameKeyManager.Instance.GetIntValue(PuzzleName.ToString()) == 1)
        {
            _boxCollider.enabled = false;
            _spriteRenderer.enabled = false;
        }
    }

    public IEnumerator Interact(Transform initiator)
    {
        Inventory inventory = initiator.gameObject.GetComponent<Inventory>();
        if (_keyItem != null )
        {
            if (inventory.HasItem(_keyItem))
            {
                GameKeyManager.Instance.SetIntValue(PuzzleName.ToString(), 1);
                yield return DialogueManager.Instance.ShowDialogueText($"使用了{_keyItem.ItemName}解开了锁。");
                UnlockAnim();
            }
            else
            {
                yield return DialogueManager.Instance.ShowDialogueText("神奇的锁，仿佛将世界与后面的空间隔绝开来。");
            }
        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText("神奇的锁，仿佛将世界与后面的空间隔绝开来。");
        }


    }

    private void UnlockAnim()
    {
        _animator.SetBool("unlock", true);
        Destroy(gameObject, 1f);
    }

}

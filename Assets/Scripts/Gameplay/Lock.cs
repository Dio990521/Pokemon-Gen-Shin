using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour, InteractableObject
{
    [SerializeField] private ItemBase _keyItem;
    [SerializeField] private int _total;
    private Animator _animator;

    public PuzzleName PuzzleName;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (GameKeyManager.Instance.GetIntValue(PuzzleName.ToString()) == 1)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
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
                yield return DialogueManager.Instance.ShowDialogueText($"ʹ����{_keyItem.ItemName}�⿪������");
                UnlockAnim();
            }
            else
            {
                yield return DialogueManager.Instance.ShowDialogueText("����������·����������Ŀռ����������");
            }
        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText("����������·����������Ŀռ����������");
        }


    }

    private void UnlockAnim()
    {
        _animator.SetBool("unlock", true);
        Destroy(gameObject, 1f);
    }

}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour, InteractableObject, ISavable
{
    [SerializeField] private ItemBase _keyItem;
    [SerializeField] private int _total;
    private int _cur;
    private Animator _animator;
    private bool _unlock;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _unlock = false;
        _cur = 0;
        GameEventManager.Instance.AddEventListener("SandTower", CheckUnlock);
    }

    private void OnDestroy()
    {
        GameEventManager.Instance.RemoveEvent("SandTower", CheckUnlock);
    }


    public IEnumerator Interact(Transform initiator)
    {
        Inventory inventory = initiator.gameObject.GetComponent<Inventory>();
        if (_keyItem != null )
        {
            if (inventory.HasItem(_keyItem))
            {
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

    private void CheckUnlock()
    {
        if (_cur == _total-1)
        {
            UnlockAnim();
        }
        else
        {
            ++_cur;
        }
    }

    public object CaptureState()
    {
        return _unlock;
    }


    public void RestoreState(object state)
    {
        _unlock = (bool)state;
        if (_unlock)
        {
            Destroy(gameObject);
        }
    }


}

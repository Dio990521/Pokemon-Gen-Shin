using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour, InteractableObject, ISavable
{
    [SerializeField] private ItemBase _key;
    private Animator _animator;
    private bool _unlock;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _unlock = false;
    }
    public IEnumerator Interact(Transform initiator)
    {
        Inventory inventory = initiator.gameObject.GetComponent<Inventory>();
        if (inventory.HasItem(_key))
        {
            yield return DialogueManager.Instance.ShowDialogueText("使用了火箭队的钥匙解开了锁。");
            _animator.SetBool("unlock", true);
            Destroy(gameObject, 1f);
        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText("神奇的锁，仿佛将山洞后的空间隔绝开来。");
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

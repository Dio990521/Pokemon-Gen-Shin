using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandTowerSwitch : MonoBehaviour, InteractableObject, ISavable
{
    [SerializeField] private ItemBase _keyItem;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public Action OnPuzzleChange;

    private bool _isActive;

    private void Awake()
    {
        _isActive = false;
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (!_isActive)
        {
            Inventory inventory = initiator.gameObject.GetComponent<Inventory>();

            if (inventory.HasItem(_keyItem))
            {
                yield return DialogueManager.Instance.ShowDialogueText($"将{_keyItem.ItemName}放在了壁槽内。");
                _isActive = true;
                _spriteRenderer.sprite = _keyItem.Icon;
                OnPuzzleChange?.Invoke();
            }
            else
            {
                yield return DialogueManager.Instance.ShowDialogueText("似乎可以放置某种东西的壁槽。");
            }
        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText($"{_keyItem.ItemName}静静地嵌在了壁槽内部。");
        }
    }

    public object CaptureState()
    {
        return _isActive;
    }

    public void RestoreState(object state)
    {
        _isActive = (bool)state;
        if (_isActive)
        {
            _spriteRenderer.sprite = _keyItem.Icon;
        }
    }


}

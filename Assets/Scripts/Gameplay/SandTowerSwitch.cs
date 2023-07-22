using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandTowerSwitch : MonoBehaviour, InteractableObject, ISavable
{
    [SerializeField] private ItemBase _keyItem;
    [SerializeField] private SpriteRenderer _spriteRenderer;
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
                yield return DialogueManager.Instance.ShowDialogueText($"��{_keyItem.ItemName}�����˱ڲ��ڡ�");
                _isActive = true;
                _spriteRenderer.sprite = _keyItem.Icon;
                GameEventManager.Instance.CallEvent("SandTower");
            }
            else
            {
                yield return DialogueManager.Instance.ShowDialogueText("�ƺ����Է���ĳ�ֶ����ıڲۡ�");
            }
        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText($"{_keyItem.ItemName}������Ƕ���˱ڲ��ڲ���");
        }
    }

    public object CaptureState()
    {
        return _isActive;
    }

    public void RestoreState(object state)
    {
        _isActive = (bool)state;
        if (_isActive )
        {
            _spriteRenderer.sprite = _keyItem.Icon;
        }
    }


}

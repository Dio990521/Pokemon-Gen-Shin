using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenChest : Pickup
{
    [SerializeField] private string _eventKey;
    [SerializeField] private int _total;
    private BoxCollider2D _boxCollider;
    [SerializeField] private int _cur = 0;

    protected override void Awake()
    {
        base.Awake();
        _boxCollider = GetComponent<BoxCollider2D>();
        _spriteRenderer.enabled = false;
        _boxCollider.enabled = false;
        GameEventManager.Instance.AddEventListener(_eventKey, CheckClear);

    }

    private void CheckClear()
    {
        ++_cur;
        if (_cur == _total)
        {
            StartCoroutine(DialogueManager.Instance.ShowDialogueText($"开启了所有的开关！\n周围貌似有宝藏的气息！"));
            _spriteRenderer.enabled = true;
            _boxCollider.enabled = true;
        }
        else
        {
            StartCoroutine(DialogueManager.Instance.ShowDialogueText($"这个区域好像还有别的开关。\n再去找找看吧！"));
        }
    }

}

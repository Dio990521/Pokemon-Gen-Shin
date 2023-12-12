using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenChest : Pickup
{
    [SerializeField] private List<Switch> _switches;
    [SerializeField] private int _total;
    [SerializeField] private int _cur = 0;

    protected override void Awake()
    {
        base.Awake();
        _spriteRenderer.enabled = false;
        _boxCollider.enabled = false;
        foreach (Switch sw in _switches)
        {
            sw.OnPuzzleChange += CheckClear;
        }

    }

    private void CheckClear()
    {
        ++_cur;
        if (_cur == _total)
        {
            StartCoroutine(DialogueManager.Instance.ShowDialogueText($"���������еĿ��أ�\n��Χò���б��ص���Ϣ��"));
            _spriteRenderer.enabled = true;
            _boxCollider.enabled = true;
        }
        else
        {
            StartCoroutine(DialogueManager.Instance.ShowDialogueText($"�����������б�Ŀ��ء�\n��ȥ���ҿ��ɣ�"));
        }
    }

}

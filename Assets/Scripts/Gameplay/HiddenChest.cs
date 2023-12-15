using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenChest : Pickup
{
    [SerializeField] private List<Switch> _switches;
    [SerializeField] private int _total;
    [SerializeField] private PuzzleName _puzzleName;

    protected override void Awake()
    {
        base.Awake();
        if (GameKeyManager.Instance.GetIntValue(_puzzleName.ToString()) != _total)
        {
            _spriteRenderer.enabled = false;
            _boxCollider.enabled = false;
            foreach (Switch sw in _switches)
            {
                sw.OnPuzzleChange += CheckClear;
            }
        }

    }

    private void CheckClear()
    {
        var cur = GameKeyManager.Instance.GetIntValue(_puzzleName.ToString());
        if (cur == _total-1)
        {
            StartCoroutine(DialogueManager.Instance.ShowDialogueText($"开启了所有的开关！\n周围貌似有宝藏的气息！"));
            _spriteRenderer.enabled = true;
            _boxCollider.enabled = true;
        }
        else
        {
            GameKeyManager.Instance.SetIntValue(_puzzleName.ToString(), cur+1);
            StartCoroutine(DialogueManager.Instance.ShowDialogueText($"这个区域好像还有别的开关。\n再去找找看吧！"));
        }
    }

}

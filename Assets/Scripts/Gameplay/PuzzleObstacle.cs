using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class PuzzleObstacle : MonoBehaviour
{
    [SerializeField] private List<GameObject> _switches;
    [SerializeField] private int _total;
    [SerializeField] private PuzzleName _puzzleName;
    private BoxCollider2D _boxCollider;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;


    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (GameKeyManager.Instance.GetIntValue(_puzzleName.ToString()) != _total-1)
        {
            foreach (var gameObject in _switches)
            {
                if (gameObject.TryGetComponent(out Switch sw))
                {
                    sw.OnPuzzleChange += CheckClear;
                }
                else if (gameObject.TryGetComponent(out SandTowerSwitch sandTowerSwitch))
                {
                    sandTowerSwitch.OnPuzzleChange += CheckSandTowerClear;
                }
            }
        }
        else
        {
            if (_boxCollider!= null)
            {
                _boxCollider.enabled = false;
            }
            if (_spriteRenderer != null)
            {
                _spriteRenderer.enabled = false;
            }
        }
    }

    private void CheckClear()
    {
        var cur = GameKeyManager.Instance.GetIntValue(_puzzleName.ToString());
        if (cur == _total-1)
        {
            StartCoroutine(DialogueManager.Instance.ShowDialogueText($"���������еĿ��أ�\n��Χ�ĵ���ò�Ʒ����˱仯��"));
            _boxCollider.enabled = false;
            _spriteRenderer.enabled = false;
        }
        else
        {
            GameKeyManager.Instance.SetIntValue(_puzzleName.ToString(), cur + 1);
            StartCoroutine(DialogueManager.Instance.ShowDialogueText($"�����������б�Ŀ��ء�\n��ȥ���ҿ��ɣ�"));
        }
    }

    private void CheckSandTowerClear()
    {
        var cur = GameKeyManager.Instance.GetIntValue(_puzzleName.ToString());
        if (cur == _total - 1)
        {
            StartCoroutine(Unlock());
        }
        else
        {
            GameKeyManager.Instance.SetIntValue(_puzzleName.ToString(), cur + 1);
        }
    }

    private IEnumerator Unlock()
    {
        yield return DialogueManager.Instance.ShowDialogueText($"���������еı��飡\nɰ֮�������ˣ�");
        UnlockAnim();
    }

    private void UnlockAnim()
    {
        if (_animator != null)
        {
            _animator.SetBool("unlock", true);
            Destroy(gameObject, 1f);
        }
    }

}

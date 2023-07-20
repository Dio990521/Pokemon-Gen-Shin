using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class PuzzleObstacle : MonoBehaviour, ISavable
{
    [SerializeField] private string _eventKey;
    [SerializeField] private int _total;
    private bool _isRemoved;
    private int _cur = 0;
    private BoxCollider2D _boxCollider;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        GameEventManager.Instance.AddEventListener(_eventKey, CheckClear);
    }

    private void CheckClear()
    {
        ++_cur;
        if (_cur == _total)
        {
            StartCoroutine(DialogueManager.Instance.ShowDialogueText($"开启了所有的开关！\n周围的地形貌似发生了变化！"));
            _boxCollider.enabled = false;
            _spriteRenderer.enabled = false;
            _isRemoved = true;
        }
        else
        {
            StartCoroutine(DialogueManager.Instance.ShowDialogueText($"这个区域好像还有别的开关。\n再去找找看吧！"));
        }
    }

    public object CaptureState()
    {
        return _isRemoved;
    }

    public void RestoreState(object state)
    {
        _isRemoved = (bool) state;
        if (_isRemoved)
        {
            _boxCollider.enabled = false;
            _spriteRenderer.enabled = false;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour, InteractableObject, ISavable
{
    [SerializeField] private string _eventKey;
    private BoxCollider2D _boxCollider;
    public bool Used { get; set; } = false;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    public object CaptureState()
    {
        return Used;
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (!Used)
        {
            Used = true;
            yield return DialogueManager.Instance.ShowDialogueText("你按下了开关！");
            GameEventManager.Instance.CallEvent(_eventKey);
        }
    }

    public void RestoreState(object state)
    {
        Used = (bool)state;
    }


}

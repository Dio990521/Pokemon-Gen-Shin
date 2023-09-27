using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour, InteractableObject
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public IEnumerator Interact(Transform initiator)
    {
        GameManager.Instance.StateMachine.Push(PCMenuState.I);
        yield return null;
    }
}

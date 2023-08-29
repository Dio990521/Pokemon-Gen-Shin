using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour, InteractableObject
{
    [SerializeField] private List<ItemBase> availableItems;
    private SpriteRenderer _spriteRenderer;

    public List<ItemBase> AvailableItems => availableItems;

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

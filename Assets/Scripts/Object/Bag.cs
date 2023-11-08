using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bag : MonoBehaviour, InteractableObject
{
    [SerializeField] private CutsceneName _disableAfterCutsceneName;

    private void Start()
    {
        if (_disableAfterCutsceneName != CutsceneName.None && GameKeyManager.Instance.GetBoolValue(_disableAfterCutsceneName.ToString()))
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public IEnumerator Interact(Transform initiator)
    {
        GameManager.Instance.StateMachine.Push(PokemonSelectionState.I);
        yield return null;
    }


}

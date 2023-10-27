using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bag : MonoBehaviour, InteractableObject
{
    [SerializeField] private CutsceneName _disableAfterCutsceneName;

    private void Start()
    {
        if (GameKeyManager.Instance.GetBoolValue(_disableAfterCutsceneName.ToString()))
        {
            gameObject.SetActive(false);
        }
    }

    public IEnumerator Interact(Transform initiator)
    {
        GameManager.Instance.StateMachine.Push(PokemonSelectionState.I);
        yield return null;
    }


}

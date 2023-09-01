using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonSelection : MonoBehaviour, InteractableObject
{
    [SerializeField] private PokemonSelectionUI _pokemonSelectionUI;


    public IEnumerator Interact(Transform initiator)
    {
        _pokemonSelectionUI.Show();
        yield return null;
    }

}

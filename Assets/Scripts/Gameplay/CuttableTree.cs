using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CuttableTree : MonoBehaviour, InteractableObject
{
    public IEnumerator Interact(Transform initiator)
    {
        yield return DialogueManager.Instance.ShowDialogueText("看起来可以被草系宝可梦破坏。");

        var pokemonWithCut = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.PokemonBase.Type1 == PokemonType.草);
        if (pokemonWithCut != null )
        {
            int selectedChoice = 0;
            yield return DialogueManager.Instance.ShowDialogueText($"让{pokemonWithCut.PokemonBase.PokemonName}砍断它吗？",
                choices: new List<string>() { "彳亍", "不了"},
                onChoiceSelected: (selection) => selectedChoice = selection);

            if (selectedChoice == 0)
            {
                yield return DialogueManager.Instance.ShowDialogueText($"{pokemonWithCut.PokemonBase.PokemonName}使出浑身力气的一击！");
                gameObject.SetActive(false);
            }

        }
    }
}

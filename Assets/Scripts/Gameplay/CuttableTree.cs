using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CuttableTree : MonoBehaviour, InteractableObject
{
    public IEnumerator Interact(Transform initiator)
    {
        yield return DialogueManager.Instance.ShowDialogueText("���������Ա���ϵ�������ƻ���");

        var pokemonWithCut = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.PokemonBase.Type1 == PokemonType.��);
        if (pokemonWithCut != null )
        {
            int selectedChoice = 0;
            yield return DialogueManager.Instance.ShowDialogueText($"��{pokemonWithCut.PokemonBase.PokemonName}��������",
                choices: new List<string>() { "��ء", "����"},
                onChoiceSelected: (selection) => selectedChoice = selection);

            if (selectedChoice == 0)
            {
                yield return DialogueManager.Instance.ShowDialogueText($"{pokemonWithCut.PokemonBase.PokemonName}ʹ������������һ����");
                gameObject.SetActive(false);
            }

        }
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SurfableWater : MonoBehaviour, InteractableObject
{
    public IEnumerator Interact(Transform initiator)
    {
        yield return DialogueManager.Instance.ShowDialogueText("��ϵ������Ӧ�ÿ��԰�æ��ˮ��");

        var pokemonWithSurf = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.PokemonBase.Type1 == PokemonType.��);
        if (pokemonWithSurf != null)
        {
            int selectedChoice = 0;
            yield return DialogueManager.Instance.ShowDialogueText($"��{pokemonWithSurf.PokemonBase.PokemonName}��æ��ˮ��",
                choices: new List<string>() { "��ء", "����" },
                onChoiceSelected: (selection) => selectedChoice = selection);

            if (selectedChoice == 0)
            {
                yield return DialogueManager.Instance.ShowDialogueText($"{pokemonWithSurf.PokemonBase.PokemonName}����������������Ľ��£�");

                var animator = initiator.GetComponent<CharacterAnimator>();
                var dir = new Vector3(animator.MoveX, animator.MoveY);
                var targetPos = initiator.position + dir;

                yield return initiator.DOJump(targetPos, 0.5f, 1, 0.5f).WaitForCompletion();
                animator.IsSurfing = true;
            }

        }
    }
}

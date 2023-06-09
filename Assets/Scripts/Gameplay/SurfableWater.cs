using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SurfableWater : MonoBehaviour, InteractableObject
{
    public IEnumerator Interact(Transform initiator)
    {
        yield return DialogueManager.Instance.ShowDialogueText("冰系宝可梦应该可以帮忙渡水。");

        var pokemonWithSurf = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.PokemonBase.Type1 == PokemonType.冰);
        if (pokemonWithSurf != null)
        {
            int selectedChoice = 0;
            yield return DialogueManager.Instance.ShowDialogueText($"让{pokemonWithSurf.PokemonBase.PokemonName}帮忙渡水吗？",
                choices: new List<string>() { "彳亍", "不了" },
                onChoiceSelected: (selection) => selectedChoice = selection);

            if (selectedChoice == 0)
            {
                yield return DialogueManager.Instance.ShowDialogueText($"{pokemonWithSurf.PokemonBase.PokemonName}让冷气缠绕在了你的脚下！");

                var animator = initiator.GetComponent<CharacterAnimator>();
                var dir = new Vector3(animator.MoveX, animator.MoveY);
                var targetPos = initiator.position + dir;

                yield return initiator.DOJump(targetPos, 0.5f, 1, 0.5f).WaitForCompletion();
                animator.IsSurfing = true;
            }

        }
    }
}

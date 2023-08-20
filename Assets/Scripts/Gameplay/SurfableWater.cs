using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SurfableWater : MonoBehaviour, InteractableObject, IPlayerTriggerable
{
    bool isJumpingToWater = false;

    public bool TriggerRepeatedly => true;

    public IEnumerator Interact(Transform initiator)
    {
        var animator = initiator.GetComponent<CharacterAnimator>();
        if (animator.IsSurfing || isJumpingToWater)
        {
            yield break;
        }

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

                var dir = new Vector3(animator.MoveX, animator.MoveY);
                var targetPos = initiator.position + dir;

                isJumpingToWater = true;
                yield return initiator.DOJump(targetPos, 0.5f, 1, 0.5f).WaitForCompletion();
                isJumpingToWater = false;
                animator.IsSurfing = true;
            }

        }
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        if (UnityEngine.Random.Range(1, 101) <= 5)
        {
            player.Character.Animator.IsMoving = false;
            GameManager.Instance.StartBattle(BattleTrigger.Water);
        }
    }
}

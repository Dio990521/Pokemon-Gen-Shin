using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiveWater : MonoBehaviour, InteractableObject
{
    bool isJumpingToWater = false;

    public IEnumerator Interact(Transform initiator)
    {
        var animator = initiator.GetComponent<CharacterAnimator>();
        if (isJumpingToWater)
        {
            yield break;
        }
        yield return DialogueManager.Instance.ShowDialogueText("ˮ���ƺ�����ͨ����һ������");
        yield return DialogueManager.Instance.ShowDialogueText("ˮϵ������Ӧ�ÿ��԰�æ��Ǳ��");

        var pokemonWithSurf = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.PokemonBase.Type1 == PokemonType.ˮ);
        if (pokemonWithSurf != null)
        {

            yield return DialogueManager.Instance.ShowDialogueText($"��{pokemonWithSurf.PokemonBase.PokemonName}��æǱˮ��", autoClose: false);
            ChoiceState.I.Choices = new List<string>() { "��ء", "����" };
            yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

            int selectedChoice = ChoiceState.I.Selection;

            if (selectedChoice == 0)
            {
                yield return DialogueManager.Instance.ShowDialogueText($"{pokemonWithSurf.PokemonBase.PokemonName}����������ȥ��");
                GameManager.Instance.PauseGame(true);
                var dir = new Vector3(animator.MoveX, animator.MoveY);
                var targetPos = initiator.position + dir * 2;

                isJumpingToWater = true;
                yield return initiator.DOJump(targetPos, 1.5f, 1, 0.5f).WaitForCompletion();
                yield return Teleport.StartTeleport(new Vector2(120.5f, -105.5f), GameManager.Instance.PlayerController, FacingDirection.Left, needSFX: false);
                isJumpingToWater = false;
            }

        }
    }

}

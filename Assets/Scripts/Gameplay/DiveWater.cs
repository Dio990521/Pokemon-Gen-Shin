using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiveWater : MonoBehaviour, InteractableObject, IPlayerTriggerable
{
    [SerializeField] private Vector2 _destination;
    [SerializeField] private bool _isDiving;

    bool isJumpingToWater = false;

    public bool TriggerRepeatedly => true;

    public IEnumerator Interact(Transform initiator)
    {
        var player = GameManager.Instance.PlayerController;
        var animator = initiator.GetComponent<CharacterAnimator>();
        if (isJumpingToWater)
        {
            yield break;
        }
        if (_isDiving )
        {
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
                    animator.IsJumping = true;
                    yield return initiator.DOJump(targetPos, 1.5f, 1, 0.5f).WaitForCompletion();
                    AudioManager.Instance.PlaySE(SFX.DIVE);
                    animator.IsJumping = false;
                    yield return Teleport.StartTeleport(_destination, player, FacingDirection.Down, needSFX: false);
                    isJumpingToWater = false;
                }

            }
        }
        else
        {
            player.StopMovingAnimation();
            yield return DialogueManager.Instance.ShowDialogueText("�Ϸ��ƺ��й�â����������");

            var pokemonWithSurf = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.PokemonBase.Type1 == PokemonType.ˮ);
            if (pokemonWithSurf != null)
            {
                yield return DialogueManager.Instance.ShowDialogueText($"Ҫ����ȥ��", autoClose: false);
                ChoiceState.I.Choices = new List<string>() { "��ء", "����" };
                yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

                int selectedChoice = ChoiceState.I.Selection;

                if (selectedChoice == 0)
                {
                    GameManager.Instance.PauseGame(true);
                    AudioManager.Instance.PlaySE(SFX.DIVE);
                    yield return Teleport.StartTeleport(_destination, player, FacingDirection.Right, needSFX: false);
                }
                else
                {
                    var dir = new Vector3(player.Character.Animator.MoveX, player.Character.Animator.MoveY);
                    yield return player.Character.Move(-dir, checkCollisions: false);
                }

            }
        }

    }

    public void OnPlayerTriggered(PlayerController player)
    {
        if (_isDiving) return;
        StartCoroutine(Interact(player.transform));
    }

}

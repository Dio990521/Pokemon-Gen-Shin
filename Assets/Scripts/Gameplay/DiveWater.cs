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
            yield return DialogueManager.Instance.ShowDialogueText("水下似乎可以通往另一个区域。");
            yield return DialogueManager.Instance.ShowDialogueText("水系宝可梦应该可以帮忙下潜。");

            var pokemonWithSurf = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.PokemonBase.Type1 == PokemonType.水);
            if (pokemonWithSurf != null)
            {

                yield return DialogueManager.Instance.ShowDialogueText($"让{pokemonWithSurf.PokemonBase.PokemonName}帮忙潜水吗？", autoClose: false);
                ChoiceState.I.Choices = new List<string>() { "彳亍", "不了" };
                yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

                int selectedChoice = ChoiceState.I.Selection;

                if (selectedChoice == 0)
                {
                    yield return DialogueManager.Instance.ShowDialogueText($"{pokemonWithSurf.PokemonBase.PokemonName}把你踹了下去！");
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
            yield return DialogueManager.Instance.ShowDialogueText("上方似乎有光芒照射下来。");

            var pokemonWithSurf = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.PokemonBase.Type1 == PokemonType.水);
            if (pokemonWithSurf != null)
            {
                yield return DialogueManager.Instance.ShowDialogueText($"要浮上去吗？", autoClose: false);
                ChoiceState.I.Choices = new List<string>() { "彳亍", "不了" };
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

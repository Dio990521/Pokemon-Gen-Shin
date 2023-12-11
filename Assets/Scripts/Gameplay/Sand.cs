using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sand : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private int _yOffset;

    public bool TriggerRepeatedly => false;

    public void OnPlayerTriggered(PlayerController player)
    {
        
        float dir = player.Character.Animator.MoveY;
        player.Character.Animator.IsMoving = false;
        if (dir > 0)
        {
            var pokemonWithRock = player.gameObject.GetComponent<PokemonParty>()
                .Pokemons.FirstOrDefault(p => p.PokemonBase.Type1 == PokemonType.��
                && p.Hp > 0);
            if (pokemonWithRock == null)
            {
                StartCoroutine(FallAnimate(player.transform));
            }
            else
            {
                StartCoroutine(SlideAnimate(player.transform));
            }
        }
        else
        {
            StartCoroutine(SlideAnimate(player.transform));
        }
        
    }

    private IEnumerator FallAnimate(Transform player)
    {
        GameManager.Instance.PauseGame(true);
        AudioManager.Instance.PlaySE(SFX.SLIDE_FAIL);
        yield return player.DOMoveY(player.position.y - 1, 1f).WaitForCompletion();
        GameManager.Instance.PauseGame(false);
        yield return DialogueManager.Instance.ShowDialogueText("Я����ϵ�����λ�����԰�������ɳ����");
    }

    private IEnumerator SlideAnimate(Transform player)
    {
        GameManager.Instance.PauseGame(true);
        var dir = player.GetComponent<CharacterAnimator>().MoveY;
        if (dir > 0)
        {
            AudioManager.Instance.PlaySE(SFX.SLIDE_UP);
        }
        else
        {
            AudioManager.Instance.PlaySE(SFX.SLIDE_DOWN);
        }
        yield return player.DOMoveY(player.position.y + dir * _yOffset, 1.5f).WaitForCompletion();
        GameManager.Instance.PauseGame(false);
    }

    
}

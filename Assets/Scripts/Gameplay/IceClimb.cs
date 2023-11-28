using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;

public class IceClimb : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private int _yOffset;

    public bool TriggerRepeatedly => false;

    public void OnPlayerTriggered(PlayerController player)
    {

        float dir = player.Character.Animator.MoveY;
        player.Character.Animator.IsMoving = false;
        if (dir > 0)
        {
            var pokemonWithIce = player.gameObject.GetComponent<PokemonParty>()
                .Pokemons.FirstOrDefault(p => p.PokemonBase.Type1 == PokemonType.冰
                && p.Hp > 0);
            if (pokemonWithIce == null)
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
        yield return player.DOMoveY(player.position.y - 1, 1f).WaitForCompletion();
        GameManager.Instance.PauseGame(false);
        yield return DialogueManager.Instance.ShowDialogueText("携带冰系宝可梦或许可以帮助爬上冰道。");
    }

    private IEnumerator SlideAnimate(Transform player)
    {
        GameManager.Instance.PauseGame(true);
        yield return player.DOMoveY(player.position.y + player.GetComponent<CharacterAnimator>().MoveY * _yOffset, 1.5f).WaitForCompletion();
        GameManager.Instance.PauseGame(false);
    }
}

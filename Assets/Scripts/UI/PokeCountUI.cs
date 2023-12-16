using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokeCountUI : MonoBehaviour
{
    private Animator _animator;
    public List<Image> CountBallImages;
    public Image Arrow;

    public Sprite EmptySprite;
    public Sprite ValidSprite;
    public Sprite InvalidSprite;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Init(List<Pokemon> partyPokemon)
    {
        foreach (var image in CountBallImages)
        {
            image.color = Color.white;
            image.sprite = EmptySprite;
        }
        for (int i = 0; i < partyPokemon.Count; i++)
        {
            if (partyPokemon[i].Hp > 0)
            {
                CountBallImages[i].sprite = ValidSprite;
            }
            else
            {
                CountBallImages[i].sprite = InvalidSprite;
            }
        }
    }

    public void EnterAnim()
    {
        _animator.SetBool("IsEnter", true);
        _animator.SetBool("IsExit", false);
    }

    public void ExitAnim()
    {
        _animator.SetBool("IsExit", true);
        _animator.SetBool("IsEnter", false);
    }

    public void Reshow(List<Pokemon> partyPokemon)
    {
        Init(partyPokemon);
        AudioManager.Instance.PlaySE(SFX.POKE_COUNT1);
        StartCoroutine(ReShowAnim());
    }

    private IEnumerator ReShowAnim()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveX(transform.localPosition.x + 500f, 0.6f));
        sequence.AppendInterval(1f);
        yield return sequence.WaitForCompletion();
        yield return ResetAnim();
    }

    private IEnumerator ResetAnim()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveX(transform.localPosition.x - 500f, 0.6f));
        yield return sequence.WaitForCompletion();
    }
}

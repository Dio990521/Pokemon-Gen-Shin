using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    private PokemonBase pokemonBase;
    private int level;
    [SerializeField] private bool isPlayerUnit;
    [SerializeField] private BattleHud hud;
    [SerializeField] private Sprite defaultPlayerSprite;

    public Pokemon pokemon { get; set; }

    public BattleHud Hud { get { return hud; } }

    public bool IsPlayerUnit { get { return isPlayerUnit; } }

    private Image unitSprite;
    public Image groundSprite;

    private Vector3 unitOriginPos;
    private Vector3 groundOriginPos;
    private Color originalColor;

    private void Awake()
    {
        unitSprite = GetComponent<Image>();
        unitOriginPos= unitSprite.transform.localPosition;
        groundOriginPos = groundSprite.transform.localPosition;
        originalColor = unitSprite.color;
    }

    public void SetUp(Pokemon selectedPokemon)
    {
        pokemon = selectedPokemon;
        level = pokemon.Level;
        unitSprite.sprite = isPlayerUnit? pokemon.PokemonBase.BackSprite : pokemon.PokemonBase.FrontSprite;
        unitSprite.color = originalColor;
        ResetAnimation();
        hud.SetData(selectedPokemon);
    }

    public void SetDefaultPlayerSprite()
    {
        unitSprite.sprite = defaultPlayerSprite;
    }

    public void HideHud()
    {
        hud.gameObject.SetActive(false);
    }

    public void ShowHud()
    {
        hud.gameObject.SetActive(true);
    }

    public void SetGroundImage(Sprite ground)
    {
        groundSprite.sprite = ground;
    }

    public void ChangeUnit(Pokemon selectedPokemon)
    {
        pokemon = selectedPokemon;
        level = pokemon.Level;
        unitSprite.sprite = isPlayerUnit ? pokemon.PokemonBase.BackSprite : pokemon.PokemonBase.FrontSprite;
        unitSprite.color = originalColor;
        hud.SetData(selectedPokemon);
        ResetAnimation();
        UnitChangeAnimation();
    }

    public void ResetAnimation()
    {
        unitSprite.transform.localScale = new Vector3(1f, 1f, 1f);
        if (isPlayerUnit)
        {
            unitSprite.transform.localPosition = new Vector3(-228f, -215f, 0f);
        }
        else
        {
            unitSprite.transform.localPosition = new Vector3(275f, 131f, 0f);
        }
        var sequence = DOTween.Sequence();
        sequence.Append(unitSprite.DOFade(1f, 0.01f));
    }

    public void UnitEnterAnimation()
    {
        if (isPlayerUnit)
        {
            unitSprite.transform.localPosition = new Vector3(unitOriginPos.x + 1000f, unitOriginPos.y);
            groundSprite.transform.localPosition = new Vector3(groundOriginPos.x + 1000f, groundOriginPos.y);
        }
        else
        {
            unitSprite.transform.localPosition = new Vector3(unitOriginPos.x - 1000f, unitOriginPos.y);
            groundSprite.transform.localPosition = new Vector3(groundOriginPos.x - 1000f, groundOriginPos.y);
        }

        unitSprite.transform.DOLocalMoveX(unitOriginPos.x, 4f);
        groundSprite.transform.DOLocalMoveX(groundOriginPos.x, 4f);
    }

    public void UnitChangeAnimation()
    {
        if (isPlayerUnit)
        {
            unitSprite.transform.localPosition = isPlayerUnit ?
            new Vector3(unitOriginPos.x - 500f, unitOriginPos.y) :
            new Vector3(unitOriginPos.x + 500f, unitOriginPos.y);
            unitSprite.transform.DOLocalMoveX(unitOriginPos.x, 2f);
        }
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
            sequence.Append(unitSprite.transform.DOLocalMove(
                new Vector3(unitOriginPos.x + 50f, unitOriginPos.y + 50f, 0f), 0.25f
                ));
        }
        else
        {
            sequence.Append(unitSprite.transform.DOLocalMove(
                new Vector3(unitOriginPos.x - 50f, unitOriginPos.y - 50f, 0f), 0.25f
                ));
        }
        sequence.Append(unitSprite.transform.DOLocalMove(new Vector3(unitOriginPos.x, unitOriginPos.y, 0f), 0.25f
                ));
    }

    public void PlayPerformMoveAnimation()
    {

    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(unitSprite.DOColor(Color.gray, 0.1f));
        sequence.Append(unitSprite.DOColor(originalColor, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(unitSprite.transform.DOLocalMoveY(unitOriginPos.y - 150f, 0.5f));
        sequence.Join(unitSprite.DOFade(0f, 0.5f));
    }

    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(unitSprite.DOFade(0, 0.5f));
        sequence.Join(unitSprite.transform.DOScale(new Vector3(0.3f, 0.3f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }

    public IEnumerator PlayBreakOutAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(unitSprite.DOFade(1, 0.5f));
        sequence.Join(unitSprite.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }
}

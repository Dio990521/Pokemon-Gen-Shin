using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    private PokemonBase pokemonBase;
    private int level;
    [SerializeField] private bool isPlayerUnit;
    [SerializeField] private BattleHud hud;
    [SerializeField] private Sprite defaultPlayerSprite;
    [SerializeField] private Image moveEffectSprite;

    public Pokemon pokemon { get; set; }

    public BattleHud Hud { get { return hud; } }

    public bool IsPlayerUnit { get { return isPlayerUnit; } }

    public Image MoveEffectSprite { get => moveEffectSprite; set => moveEffectSprite = value; }

    private Image unitSprite;
    public Image groundSprite;

    private Vector3 unitOriginPos;
    private Vector3 groundOriginPos;
    private Color originalColor;

    private Vector3 playerPokeballPos;

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
        if (selectedPokemon.PokemonBase.IsLargePortrait)
        {
            unitSprite.rectTransform.sizeDelta = new Vector2(800f, 500f);
        }
        else
        {
            unitSprite.rectTransform.sizeDelta = new Vector2(400f, 450f);
        }
        unitSprite.sprite = isPlayerUnit ? pokemon.PokemonBase.BackSprite : pokemon.PokemonBase.FrontSprite;
        unitSprite.SetNativeSize();
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
            unitSprite.transform.localPosition = new Vector3(275f, -94f, 0f);
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
        var color = unitSprite.color;
        color.a = 1f;
        unitSprite.color = color;
        AudioManager.Instance.PlaySE(SFX.BALL_OUT);
        unitSprite.transform.localScale = new Vector3(0f, 0f, 0f);
        unitSprite.transform.DOScale(new Vector3(1f, 1f, 1f), 1f);
    }

    public IEnumerator ThrowBallAnimation(BattleSystem battleSystem)
    {
        unitSprite.transform.localPosition = new Vector3(-228f, -215f, 0f);
        var pokeballObj = Instantiate(battleSystem.PokeballSprite, playerPokeballPos, Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();
        pokeball.sprite = pokemon.PokeballSprite;
        var sequence = DOTween.Sequence();
        sequence.Append(pokeball.transform.DOJump(unitSprite.transform.position, 4f, 1, 1.5f));
        sequence.Join(pokeball.transform.DORotate(new Vector3(0f, 0f, 360 * 20), 1.5f, RotateMode.LocalAxisAdd));
        Destroy(pokeballObj, 1.6f);
        yield return null;
    }

    public IEnumerator PlayerThrowBallAnimation(BattleSystem battleSystem, Pokemon playerFirstPokemon)
    {
        Vector3 defaultPos = unitSprite.transform.position;
        yield return unitSprite.transform.DOLocalMoveX(defaultPos.x - 600, 1.5f, false);
        yield return new WaitForSeconds(1f);
        var color = unitSprite.color;
        color.a = 0f;
        unitSprite.color = color;
        playerPokeballPos = unitSprite.transform.position;
        var pokeballObj = Instantiate(battleSystem.PokeballSprite, unitSprite.transform.position, Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();
        pokeball.sprite = playerFirstPokemon.PokeballSprite;
        var sequence = DOTween.Sequence();
        sequence.Append(pokeball.transform.DOJump(defaultPos, 4f, 1, 1.5f));
        sequence.Join(pokeball.transform.DORotate(new Vector3(0f, 0f, 360 * 20), 1.5f, RotateMode.LocalAxisAdd));
        unitSprite.transform.localPosition = defaultPos;
        Destroy(pokeballObj, 1.6f);

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

    public void PlayPerformMoveAnimation(List<Sprite> moveEffectSprites)
    {
        if (moveEffectSprites.Count > 0)
        {
            StartCoroutine(Animate(moveEffectSprites, 0.15f));
        }
    }

    private IEnumerator Animate(List<Sprite> moveEffectSprites, float frameRate)
    {
        int frame = 0;
        Color imageColor = moveEffectSprite.color;
        imageColor.a = 1f;
        moveEffectSprite.color = imageColor;
        while (frame >= 0 && frame < moveEffectSprites.Count)
        {
            moveEffectSprite.sprite = moveEffectSprites[frame];
            yield return new WaitForSeconds(frameRate);
            frame++;
        }
        imageColor.a = 0f;
        moveEffectSprite.color = imageColor;
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

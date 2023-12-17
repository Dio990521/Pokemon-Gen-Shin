using DG.Tweening;
using System;
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
    public Image BoostEffect;
    public Action<int, bool> OnBoostEffect;
    [SerializeField] private Sprite _boostSprite;
    [SerializeField] private Sprite _boostDownSprite;

    public int CurAccumulatePower;
    public bool IsAccumulating;

    public bool OnSecondPhase = false;

    public Pokemon pokemon { get; set; }

    public BattleHud Hud { get { return hud; } }

    public bool IsPlayerUnit { get { return isPlayerUnit; } }

    public Image MoveEffectSprite { get => moveEffectSprite; set => moveEffectSprite = value; }
    public Image TrainerSprite { get => _trainerSprite; set => _trainerSprite = value; }
    public Image PokemonSprite { get => _pokemonSprite; set => _pokemonSprite = value; }
    public Sprite PokeballOpenSprite { get => _pokeballOpenSprite; set => _pokeballOpenSprite = value; }
    public Image Pokeball { get => _pokeball; set => _pokeball = value; }
    public Sprite PokeballCloseSprite { get => _pokeballCloseSprite; set => _pokeballCloseSprite = value; }

    [SerializeField] private Image _trainerSprite;
    [SerializeField] private Image _pokemonSprite;
    [SerializeField] private Image _pokeball;
    [SerializeField] private Sprite _pokeballOpenSprite;
    [SerializeField] private Sprite _pokeballCloseSprite;

    public PokeCountUI PokeCountUI;

    public Image groundSprite;

    private Vector3 unitOriginPos;
    private Vector3 groundOriginPos;
    private Color originalColor;

    private Vector3 playerPokeballPos;

    private void Awake()
    {
        unitOriginPos = transform.localPosition;
        groundOriginPos = groundSprite.transform.localPosition;
        originalColor = _pokemonSprite.color;
        OnBoostEffect += PlayBoostAnim;
    }

    public void SetUp(Pokemon selectedPokemon)
    {
        pokemon = selectedPokemon;
        OnSecondPhase = false;
        CurAccumulatePower = 0;
        level = pokemon.Level;
        if (selectedPokemon.PokemonBase.IsLargePortrait)
        {
            _pokemonSprite.rectTransform.sizeDelta = new Vector2(800f, 500f);
        }
        else
        {
            _pokemonSprite.rectTransform.sizeDelta = new Vector2(400f, 450f);
        }
        _pokemonSprite.sprite = isPlayerUnit ? pokemon.PokemonBase.BackSprite : pokemon.PokemonBase.FrontSprite;
        _pokemonSprite.SetNativeSize();
        _pokemonSprite.color = originalColor;
        ResetAnimation();
        hud.SetData(selectedPokemon, isPlayerUnit);
    }

    public void PlayBoostAnim(int boost, bool wait)
    {
        BoostEffect.gameObject.SetActive(true);
        BoostEffect.rectTransform.localPosition = new Vector2(0f, 0f);
        var sequence = DOTween.Sequence();
        if (boost > 0)
        {
            BoostEffect.sprite = _boostSprite;
            sequence.Append(BoostEffect.DOFade(0f, 0.001f));
            sequence.Append(BoostEffect.transform.DOLocalMoveY(500f, 2f));
            var sequence2 = DOTween.Sequence();
            sequence2.Append(BoostEffect.DOFade(0.7f, 0.8f));
            sequence2.Append(BoostEffect.DOFade(0f, 0.8f));
            sequence.Join(sequence2);

            sequence.OnComplete(() => 
            {
                BoostEffect.gameObject.SetActive(false);
            });

        }
        else if (boost < 0)
        {
            if (wait)
            {
                sequence.AppendInterval(0.5f);
            }
            BoostEffect.sprite = _boostDownSprite;
            sequence.Append(BoostEffect.DOFade(0f, 0.001f));
            sequence.Append(BoostEffect.transform.DOLocalMoveY(-500f, 2f));
            var sequence2 = DOTween.Sequence();
            sequence2.Append(BoostEffect.DOFade(0.7f, 0.8f));
            sequence2.Append(BoostEffect.DOFade(0f, 0.8f));
            sequence.Join(sequence2);

            sequence.OnComplete(() =>
            {
                BoostEffect.gameObject.SetActive(false);
            });
        }
    }

    public void SetNewTrainerPokemon(Pokemon selectedPokemon)
    {
        pokemon = selectedPokemon;
        level = pokemon.Level;
        if (selectedPokemon.PokemonBase.IsLargePortrait)
        {
            _pokemonSprite.rectTransform.sizeDelta = new Vector2(800f, 500f);
        }
        else
        {
            _pokemonSprite.rectTransform.sizeDelta = new Vector2(400f, 450f);
        }
        _pokemonSprite.sprite = isPlayerUnit ? pokemon.PokemonBase.BackSprite : pokemon.PokemonBase.FrontSprite;
        _pokemonSprite.SetNativeSize();
        _pokemonSprite.color = originalColor;
        _pokemonSprite.rectTransform.localPosition = new Vector3(0f, 10f, 0f);
        var sequence = DOTween.Sequence();
        sequence.Append(_pokemonSprite.DOFade(1f, 0.01f));
    }

    public void SetDefaultPlayerSprite()
    {
        _pokemonSprite.sprite = defaultPlayerSprite;
    }

    public void HideHud()
    {
        hud.gameObject.SetActive(false);
    }

    public void ShowHud()
    {
        hud.gameObject.SetActive(true);
        hud.ClearBuffs();
    }

    public void SetGroundImage(Sprite ground)
    {
        groundSprite.sprite = ground;
    }

    public void ChangeUnit(Pokemon selectedPokemon, bool trainer=false)
    {
        pokemon = selectedPokemon;
        level = pokemon.Level;
        if (selectedPokemon.PokemonBase.IsLargePortrait)
        {
            _pokemonSprite.rectTransform.sizeDelta = new Vector2(800f, 500f);
        }
        else
        {
            _pokemonSprite.rectTransform.sizeDelta = new Vector2(400f, 450f);
        }
        _pokemonSprite.sprite = isPlayerUnit ? pokemon.PokemonBase.BackSprite : pokemon.PokemonBase.FrontSprite;
        _pokemonSprite.SetNativeSize();
        _pokemonSprite.color = originalColor;
        hud.SetData(selectedPokemon, isPlayerUnit);
        ResetAnimation(trainer);
        UnitChangeAnimation();
    }

    public void ResetAnimation(bool trainer=false)
    {
        _pokemonSprite.transform.localScale = new Vector3(1f, 1f, 1f);
        if (isPlayerUnit)
        {
            transform.localPosition = new Vector3(-233.41f, -185f, 0f);
        }
        else
        {
            transform.localPosition = new Vector3(275f, -94f, 0f);
            _pokemonSprite.rectTransform.localPosition = new Vector3(0f, 10f, 0f);
            if (!trainer)
            {
                _trainerSprite.rectTransform.anchoredPosition = new Vector3(0f, -75f, 0f);
            }
        }
        var sequence = DOTween.Sequence();
        sequence.Append(_pokemonSprite.DOFade(1f, 0.01f));
    }

    public void UnitEnterAnimation()
    {
        if (isPlayerUnit)
        {
            transform.localPosition = new Vector3(unitOriginPos.x + 1000f, unitOriginPos.y);
            groundSprite.transform.localPosition = new Vector3(groundOriginPos.x + 1000f, groundOriginPos.y);
        }
        else
        {
            transform.localPosition = new Vector3(unitOriginPos.x - 1000f, unitOriginPos.y);
            groundSprite.transform.localPosition = new Vector3(groundOriginPos.x - 1000f, groundOriginPos.y);
        }

        transform.DOLocalMoveX(unitOriginPos.x, 4f);
        groundSprite.transform.DOLocalMoveX(groundOriginPos.x, 4f);
    }

    public IEnumerator MoveTrainerImage(float xOffset, bool isRight, float time=2f)
    {
        if (isRight)
        {
            yield return TrainerSprite.rectTransform.DOLocalMoveX(TrainerSprite.rectTransform.anchoredPosition.x + xOffset, time);
        }
        else
        {
            yield return TrainerSprite.rectTransform.DOLocalMoveX(TrainerSprite.rectTransform.anchoredPosition.x - xOffset, time);

        }
    }

    public void UnitChangeAnimation()
    {
        var color = _pokemonSprite.color;
        color.a = 1f;
        _pokemonSprite.color = color;
        AudioManager.Instance.PlaySE(SFX.BALL_OUT);
        _pokemonSprite.transform.localScale = new Vector3(0f, 0f, 0f);
        _pokemonSprite.transform.DOScale(new Vector3(1f, 1f, 1f), 1f);
    }

    public IEnumerator ThrowBallAnimation(BattleSystem battleSystem, Pokemon playerPokemon)
    {
        _pokemonSprite.transform.localPosition = new Vector3(-228f, -215f, 0f);
        var pokeballObj = Instantiate(battleSystem.PokeballSprite, playerPokeballPos, Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();
        pokeball.sprite = GameManager.Instance.GetPokeSprite(playerPokemon.PokeballSpriteType);
        var sequence = DOTween.Sequence();
        sequence.Append(pokeball.transform.DOJump(_pokemonSprite.transform.position, 4f, 1, 1.5f));
        sequence.Join(pokeball.transform.DORotate(new Vector3(0f, 0f, 360 * 20), 1.5f, RotateMode.LocalAxisAdd));
        Destroy(pokeballObj, 1.6f);
        yield return null;
    }

    public IEnumerator PlayerThrowBallAnimation(BattleSystem battleSystem, Pokemon playerFirstPokemon)
    {
        Vector3 defaultPos = _pokemonSprite.transform.position;
        yield return _pokemonSprite.transform.DOLocalMoveX(defaultPos.x - 600, 1.5f, false);
        yield return new WaitForSeconds(1f);
        var color = _pokemonSprite.color;
        color.a = 0f;
        _pokemonSprite.color = color;
        playerPokeballPos = _pokemonSprite.transform.position;
        var pokeballObj = Instantiate(battleSystem.PokeballSprite, _pokemonSprite.transform.position, Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();
        pokeball.sprite = GameManager.Instance.GetPokeSprite(playerFirstPokemon.PokeballSpriteType);
        var sequence = DOTween.Sequence();
        sequence.Append(pokeball.transform.DOJump(defaultPos, 4f, 1, 1.5f));
        sequence.Join(pokeball.transform.DORotate(new Vector3(0f, 0f, 360 * 20), 1.5f, RotateMode.LocalAxisAdd));
        _pokemonSprite.transform.localPosition = defaultPos;
        Destroy(pokeballObj, 1.6f);

    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        var pokemonOriginPos = _pokemonSprite.rectTransform.localPosition;
        if (isPlayerUnit)
        {
            sequence.Append(_pokemonSprite.rectTransform.DOLocalMove(
                new Vector3(pokemonOriginPos.x + 50f, pokemonOriginPos.y + 50f, 0f), 0.25f
                ));
        }
        else
        {
            sequence.Append(_pokemonSprite.rectTransform.DOLocalMove(
                new Vector3(pokemonOriginPos.x - 50f, pokemonOriginPos.y - 50f, 0f), 0.25f
                ));
        }
        sequence.Append(_pokemonSprite.transform.DOLocalMove(new Vector3(pokemonOriginPos.x, pokemonOriginPos.y, 0f), 0.25f
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
        StartCoroutine(hud.HudShakeAnim());
        var sequence = DOTween.Sequence();
        sequence.Append(_pokemonSprite.DOColor(Color.gray, 0.1f));
        sequence.Append(_pokemonSprite.DOColor(originalColor, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        if (IsPlayerUnit)
        {
            sequence.Append(_pokemonSprite.transform.DOLocalMoveY(unitOriginPos.y - 150f, 0.5f));
        }
        else
        {
            sequence.Append(_pokemonSprite.rectTransform.DOLocalMoveY(_pokemonSprite.rectTransform.localPosition.y - 150f, 0.5f));
        }
        sequence.Join(_pokemonSprite.DOFade(0f, 0.5f));
    }

    public IEnumerator PlayCaptureAnimation(Vector3 pokeballPos)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(_pokemonSprite.DOFade(0, 0.5f));
        sequence.Join(_pokemonSprite.transform.DOScale(new Vector3(0.3f, 0.3f, 1f), 0.5f));
        sequence.Join(_pokemonSprite.transform.DOMoveY(pokeballPos.y, 0.5f));
        yield return sequence.WaitForCompletion();
    }

    public IEnumerator PlayBreakOutAnimation(Vector3 originPos)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(_pokemonSprite.DOFade(1, 0.5f));
        sequence.Join(_pokemonSprite.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
        sequence.Join(_pokemonSprite.transform.DOMoveY(originPos.y, 0.5f));
        yield return sequence.WaitForCompletion();
    }
}

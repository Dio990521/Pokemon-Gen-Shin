using DG.Tweening;
using Game.Tool.Singleton;
using PokeGenshinUtils.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionState : State<GameManager>
{
    [SerializeField] private GameObject evolutionUI;
    [SerializeField] private Image pokemonImage;
    [SerializeField] private Image mask;

    public static EvolutionState I { get; private set; }

    private void Awake()
    {
        I = this;
    }


    public IEnumerator Evolve(Pokemon pokemon, Evolution evolution)
    {
        var gameManager = GameManager.Instance;
        gameManager.StateMachine.Push(this);

        evolutionUI.SetActive(true);

        pokemonImage.sprite = pokemon.PokemonBase.FrontSprite;
        pokemonImage.SetNativeSize();
        var oldPokemon = pokemon.PokemonBase;
        AudioManager.Instance.PlayMusicVolume(BGM.EVOLUTION);
        yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.PokemonBase.PokemonName}正在进化！", waitForInput: false, autoClose: false);
        yield return mask.DOFade(1f, 1.5f).WaitForCompletion();
        int count = 1;
        bool scaleLarge = false;
        bool isNewPokemonSprite = true;
        for (float i = 2f;;)
        {
            count++;
            if (scaleLarge)
            {
                if (isNewPokemonSprite)
                {
                    yield return ScaleLargeAnim(evolution.EvolvesInto, i);
                }
                else
                {
                    yield return ScaleLargeAnim(oldPokemon, i);
                }
                isNewPokemonSprite = !isNewPokemonSprite;
            }
            else
            {
                yield return ScaleSmallAnim(i);
            }
            scaleLarge = !scaleLarge;
            if (count < 20)
            {
                i -= (0.03f * count);
                i = Mathf.Clamp(i, 0.1f, 2f);
            }
            else
            {
                i = 0.1f;
                if (count > 31) break;
            }
        }
        pokemonImage.sprite = evolution.EvolvesInto.FrontSprite;
        pokemonImage.SetNativeSize();
        yield return pokemonImage.transform.DOScale(new Vector3(1, 1, 1), 2f).WaitForCompletion();
        yield return mask.DOFade(0f, 2f).WaitForCompletion();
        AudioManager.Instance.PlayMusicVolume(BGM.EVOLUTION_CONGRAT, false);
        pokemon.Evolve(evolution);
        yield return DialogueManager.Instance.ShowDialogueText($"{oldPokemon.PokemonName}成功进化为\n{pokemon.PokemonBase.PokemonName}了！");
        yield return DialogueManager.Instance.ShowDialogueText($"基础属性得到了提升！");

        evolutionUI.SetActive(false);

        gameManager.PartyScreen.SetPartyData();
        AudioManager.Instance.PlayMusicVolume(gameManager.CurrentScene.SceneMusic, fade: true);

        gameManager.StateMachine.Pop();
    }

    public IEnumerator ScaleSmallAnim(float duration)
    {
        yield return pokemonImage.transform.DOScale(new Vector3(0, 0, 0), duration).WaitForCompletion();
    }

    public IEnumerator ScaleLargeAnim(PokemonBase pokemon, float duration)
    {
        pokemonImage.sprite = pokemon.FrontSprite;
        pokemonImage.SetNativeSize();
        yield return pokemonImage.transform.DOScale(new Vector3(1, 1, 1), duration).WaitForCompletion();
    }
}

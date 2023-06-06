using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFX
{
    CONFIRM,
    ATTACK,
    LOW_ATTACK,
    EFFICIENT_ATTACK,
    FAINTED,
    BALL_OUT,
    ESCAPE,
    THROW_BALL
}

public enum BGM
{
    LITTLEROOT_TOWN,
    BATTLE_WILD_POKEMON,
    VICTORY_WILD_POKEMON,
    TRAINER_EYE_MEET_YOUNG,
    VICTORY_TRAINER,
    BATTLE_TRAINER,
    XUMENG_FOREST,
    NONE
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioSource sfxPlayer;

    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private AudioClip[] sfxClips;

    [SerializeField] private float fadeDuration = 0.75f;
    private float originalMusicVol;

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    private void Start()
    {
        originalMusicVol = musicPlayer.volume;
    }

    public void PlayMusic(BGM id, bool loop=true, bool fade=false)
    {
        if (musicPlayer.clip == musicClips[(int)id])
        {
            return;
        }
        StartCoroutine(PlayerMusicAsync(id, loop, fade));
    }

    public void PlayMusic(BGM id, float volume)
    {
        musicPlayer.Stop();
        AudioClip clip = musicClips[(int)id];
        musicPlayer.clip = clip;
        musicPlayer.volume = volume;
        musicPlayer.Play();
    }

    public void StopMusic()
    {
        musicPlayer.Stop();
    }

    public void PlaySE(SFX id)
    {
        sfxPlayer.pitch = 1f;
        AudioClip clip = sfxClips[(int)id];
        sfxPlayer.PlayOneShot(clip);
    }

    public void PlaySE(SFX id, float volume)
    {
        sfxPlayer.pitch = 1f;
        AudioClip clip = sfxClips[(int)id];
        sfxPlayer.PlayOneShot(clip, volume);
    }

    private IEnumerator PlayerMusicAsync(BGM id, bool loop, bool fade)
    {
        if (fade)
        {
            yield return musicPlayer.DOFade(0, fadeDuration).WaitForCompletion();
        }

        musicPlayer.clip = musicClips[(int)id]; ;
        musicPlayer.loop = loop;
        musicPlayer.Play();

        if (fade)
        {
            yield return musicPlayer.DOFade(originalMusicVol, fadeDuration).WaitForCompletion();
        }
    }

}
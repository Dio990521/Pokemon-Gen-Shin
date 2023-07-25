using DG.Tweening;
using Game.Tool.Singleton;
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
    THROW_BALL,
    JUMP,
    EXP_UP,
    GO_OUT,
    LEVEL_UP,
    NO,
    OPEN_DOOR,
    SURPRISE,
    TRUCK,
    TRUCK_END,
    SWITCH,
    CHEST,
    HEALTH_CENTER_IN,
    LEVEL_UP2,
    OBTAIN_TM,
    OBTAIN_ITEM,
    OBTAIN_BERRY,
    ENTRY_CALL,
    POKEMON_HEAL,
    MENU,
    TELEPORT,
    ACTIVATE_TELEPORT
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
    MIDE_BEACH,
    TIWATE,
    LIDAO_DESERT,
    NATA_MOUNTAIN,
    GYM,
    SHOP,
    LOBBY,
    ARENA,
    POKE_CENTER,
    LAB,
    DCZ,
    ABANDONED_SHIP,
    SAND_TOWER,
    CAVE,
    NONE
}

public class AudioManager : Singleton<AudioManager>
{

    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioSource sfxPlayer;

    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private AudioClip[] sfxClips;

    [SerializeField] private float fadeDuration = 0.75f;
    private float originalMusicVol;

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

    public void PlaySE(SFX id, bool pauseMusic=false)
    {
        AudioClip clip = sfxClips[(int)id];
        if (pauseMusic)
        {
            musicPlayer.Pause();
            StartCoroutine(UnPauseMusic(clip.length * 0.75f));
        }
        sfxPlayer.pitch = 1f;
        
        sfxPlayer.PlayOneShot(clip);
    }

    public void PlaySE(SFX id, float volume, bool pauseMusic = false)
    {
        sfxPlayer.pitch = 1f;
        AudioClip clip = sfxClips[(int)id];
        sfxPlayer.PlayOneShot(clip, volume);
    }

    private IEnumerator UnPauseMusic(float delay)
    {
        yield return new WaitForSeconds(delay);
        musicPlayer.volume = 0;
        musicPlayer.UnPause();
        musicPlayer.DOFade(originalMusicVol, fadeDuration);  
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

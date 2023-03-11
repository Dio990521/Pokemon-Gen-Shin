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
    BALL_OUT
}

public enum BGM
{
    LITTLEROOT_TOWN,
    BATTLE_WILD_POKEMON,
    VICTORY_WILD_POKEMON,
    TRAINER_EYE_MEET_YOUNG,
    VICTORY_TRAINER,
    BATTLE_TRAINER
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource musicPlayer;
    public AudioSource soundPlayer;

    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private AudioClip[] sfxClips;

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

    public void PlayMusic(BGM id)
    {
        musicPlayer.Stop();
        AudioClip clip = musicClips[(int)id];
        musicPlayer.clip = clip;
        musicPlayer.Play();
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
        soundPlayer.pitch = 1f;
        AudioClip clip = sfxClips[(int)id];
        soundPlayer.PlayOneShot(clip);
    }

    public void PlaySE(SFX id, float volume)
    {
        soundPlayer.pitch = 1f;
        AudioClip clip = sfxClips[(int)id];
        soundPlayer.PlayOneShot(clip, volume);
    }

}

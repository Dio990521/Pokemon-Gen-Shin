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
    NEW_GAME,
    DELETE_MOVE,
    SWITCH,
    CHEST,
    HEALTH_CENTER_IN,
    OBTAIN_TM,
    OBTAIN_ITEM,
    OBTAIN_BERRY,
    POKEMON_HEAL,
    MENU,
    TELEPORT,
    ACTIVATE_TELEPORT,
    CANCEL,
    PC_ON,
    PC_OFF,
    PC_OPERATE,
    LOCK,
    BALL_BOUNCE,
    BALL_SHAKE,
    DIVE,
    BEAST_YE,
    BEAST_HE,
    BEAST_A,
    BEAST_YARIMASU,
    BEAST_YIGE,
    OBTAIN_BADGE,
    RECEIVE_POKEMON,
    BUY,
    BOOST,
    BOOST_DOWN,
    ERROR,
    SHUT_DOWN,
    MONEY,
    CURSOR,
    CUT_TREE,
    USE_PAIMENG,
    EAT_PAIMENG,
    PAR,
    FIND_WEAKPOINT,
    BALL_FALLING,
    FIND_STRONGPOINT,
    BRN,
    SLP,
    FRZ,
    JIEJING,
    ZHANFANG,
    CFS,
    PSN,
    SLIDE_UP,
    SLIDE_DOWN,
    SLIDE_FAIL,
    USE_RECOVERY,
    HEAL_MOVE,
    POKE_COUNT1,
    POKE_COUNT2,
    BADGE_CLEAR,
    USE_BOOST,
    XUANNAGE,
    LEARN_TM
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
    CATCH_POKEMON,
    INTRODUCTION,
    OPENNING_MOVIE,
    OPENNING,
    TITLE,
    XIAOYAO,
    HELP_ME,
    TRAINER_EYE_MEET_LASS,
    TEAM_MAGMA_APPEARS,
    BATTLE_TEAM_AQUA_MAGMA,
    VICTORY_TEAM_AQUA_MAGMA,
    TRAINER_EYE_MEET_TUBER_G,
    TRAINER_EYE_MEET_HIKER,
    BATTLE_GYM_LEADER,
    VICTORY_GYM_LEADER,
    TRAINER_EYE_MEET_GENTLEMAN,
    BATTLE_XIAOYAO,
    TRAINER_EYE_MEET_PSYCHIC,
    TRAINER_EYE_MEET_HEX_MANIAC,
    TEAM_AQUA_APPEARS,
    BATTLE_AQUA_MAGMA_LEADER,
    BATTLE_SUPER_ANCIENT_POKEMON,
    TRAINER_EYE_MEET_SWIMMER_G,
    BATTLE_REGIROCK_REGICE_REGISTEEL,
    TRAINER_EYE_MEET_COOLTRAINER,
    ELITE_FOUR_APPEAR,
    BATTLE_ELITE,
    CHAMPION,
    BATTTLE_CHAMPION,
    VICTORY_CHAMPION,
    EVOLUTION,
    EVOLUTION_CONGRAT,
    ENDING_THEME,
    THE_END,
    NONE,
}

public class AudioManager : Singleton<AudioManager>
{

    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioSource sfxPlayer;

    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private AudioClip[] sfxClips;
    [SerializeField] private AudioClip[] tiwateClips;

    [SerializeField] private float fadeDuration = 0.75f;
    public bool IsPlayingTiwate;

    private bool _isPausing;

    public AudioSource MusicPlayer { get => musicPlayer; set => musicPlayer = value; }
    public AudioSource SfxPlayer { get => sfxPlayer; set => sfxPlayer = value; }


    public void ChangeMusicPlayerVol(float volume)
    {
        musicPlayer.volume = volume;
    }

    public void ChangeSfxPlayerVol(float volume)
    {
        sfxPlayer.volume = volume;
    }

    public void PlayTiwateMusic()
    {
        int index = Random.Range(0, tiwateClips.Length);
        if (IsPlayingTiwate)
        {
            return;
        }
        IsPlayingTiwate = true;
        StartCoroutine(PlayerTiwateMusicAsync(index, true, true));
    }

    public void PlayMusicVolume(BGM id, bool loop=true, bool fade=false, float volume = -1f)
    {
        if (musicPlayer.clip == musicClips[(int)id])
        {
            return;
        }
        if (volume < 0)
        {
            volume = OptionState.I.OptionMenuUI.BGMSlider.value;
        }
        StartCoroutine(PlayerMusicAsync(id, volume, loop, fade));
    }

    public void PlayMusicVolume(AudioClip bgm, bool loop = true, bool fade = false, float volume = -1f)
    {
        if (musicPlayer.clip == bgm)
        {
            return;
        }
        if (volume < 0)
        {
            volume = OptionState.I.OptionMenuUI.BGMSlider.value;
        }
        StartCoroutine(PlayerMusicAsync(bgm, volume, loop, fade));
    }

    public void PlayMusic(BGM id, float fadeTime, bool loop = true, bool fade = true)
    {
        if (musicPlayer.clip == musicClips[(int)id])
        {
            return;
        }
        StartCoroutine(PlayerMusicAsync(id, loop, fade, fadeTime));
    }

    public IEnumerator StopMusic(bool fade=false)
    {
        if (fade)
        {
            yield return musicPlayer.DOFade(0, fadeDuration).WaitForCompletion();
        }
        musicPlayer.Stop();
    }

    public void PauseMusic(float delay)
    {
        musicPlayer.Pause();
        StartCoroutine(UnPauseMusic(delay));
    }

    public void StopSE()
    {
        sfxPlayer.Stop();
    }

    public void PlaySE(SFX id, bool pauseMusic=false)
    {
        AudioClip clip = sfxClips[(int)id];
        if (sfxPlayer.clip == clip)
        {
            return;
        }
        if (pauseMusic)
        {
            musicPlayer.Pause();
            StartCoroutine(UnPauseMusic(clip.length * 0.75f));
        }
        sfxPlayer.pitch = 1f;
        sfxPlayer.volume = OptionState.I.OptionMenuUI.SFXSlider.value;
        sfxPlayer.PlayOneShot(clip, 1f);
    }

    public void PlaySEClip(AudioClip sfx, float volume=-1f, bool pauseMusic = false)
    {
        if (sfxPlayer.clip == sfx)
        {
            return;
        }
        if (pauseMusic)
        {
            musicPlayer.Pause();
            StartCoroutine(UnPauseMusic(sfx.length * 0.75f));
        }
        sfxPlayer.pitch = 1f;
        if (volume < 0)
        {
            volume = OptionState.I.OptionMenuUI.SFXSlider.value;
        }
        sfxPlayer.volume = volume;
        sfxPlayer.PlayOneShot(sfx, 1f);
    }

    private IEnumerator UnPauseMusic(float delay)
    {
        if (_isPausing) yield break;
        _isPausing = true;
        yield return new WaitForSeconds(delay);
        var prevVolume = musicPlayer.volume;
        musicPlayer.volume = 0;
        musicPlayer.UnPause();
        yield return musicPlayer.DOFade(prevVolume, fadeDuration);
        _isPausing = false;
    }

    private IEnumerator PlayerMusicAsync(BGM id, float volume, bool loop, bool fade)
    {
        if (fade)
        {
            yield return musicPlayer.DOFade(0, fadeDuration).WaitForCompletion();
        }

        musicPlayer.clip = musicClips[(int)id];
        musicPlayer.volume = volume;
        musicPlayer.loop = loop;
        musicPlayer.Play();

        if (fade)
        {
            yield return musicPlayer.DOFade(OptionState.I.OptionMenuUI.BGMSlider.value, fadeDuration).WaitForCompletion();
        }
    }

    private IEnumerator PlayerMusicAsync(AudioClip music, float volume, bool loop, bool fade)
    {
        if (fade)
        {
            yield return musicPlayer.DOFade(0, fadeDuration).WaitForCompletion();
        }

        musicPlayer.clip = music;
        musicPlayer.volume = volume;
        musicPlayer.loop = loop;
        musicPlayer.Play();

        if (fade)
        {
            yield return musicPlayer.DOFade(volume, fadeDuration).WaitForCompletion();
        }
    }

    private IEnumerator PlayerTiwateMusicAsync(int id, bool loop, bool fade)
    {
        if (fade)
        {
            yield return musicPlayer.DOFade(0, fadeDuration).WaitForCompletion();
        }

        musicPlayer.clip = tiwateClips[id];
        musicPlayer.loop = loop;
        musicPlayer.Play();

        if (fade)
        {
            yield return musicPlayer.DOFade(OptionState.I.OptionMenuUI.BGMSlider.value, fadeDuration).WaitForCompletion();
        }
    }

    private IEnumerator PlayerMusicAsync(BGM id, bool loop, bool fade, float fadeTime)
    {
        if (fade)
        {
            yield return musicPlayer.DOFade(0, fadeTime).WaitForCompletion();
        }

        musicPlayer.clip = musicClips[(int)id];
        musicPlayer.loop = loop;
        musicPlayer.Play();

        if (fade)
        {
            yield return musicPlayer.DOFade(OptionState.I.OptionMenuUI.BGMSlider.value, fadeTime).WaitForCompletion();
        }
    }

}

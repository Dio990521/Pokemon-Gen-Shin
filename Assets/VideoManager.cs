using Game.Tool.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager : Singleton<VideoManager>
{
    private VideoPlayer _videoPlayer;

    [SerializeField] private VideoClip[] _videoClips;
    [SerializeField] private RawImage _videoTexture;

    private void Start()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
    }

    public IEnumerator PlayOpenning()
    {
        _videoPlayer.Stop();
        _videoTexture.gameObject.SetActive(true);
        _videoPlayer.clip = _videoClips[(int)VIDEO.OPENNING];
        _videoPlayer.isLooping = false;
        GameManager.Instance.TitleUI.SetActive(false);
        yield return new WaitForSeconds(2f);
        _videoPlayer.Play();
        yield return Fader.FadeOut(1f);
        while (_videoPlayer.isPlaying)
        {
            yield return new WaitForFixedUpdate(); //跟FixedUpdate 一样根据固定帧 更新
        }
        yield return Fader.FadeIn(0.5f);
        _videoTexture.gameObject.SetActive(false);
        GameManager.Instance.StateMachine.ChangeState(FreeRoamState.I);
        GameManager.Instance.PlayerController.transform.localPosition = new Vector3(-8.5f, -22.35f);
        GameManager.Instance.PlayerController.Character.Animator.SetFacingDirection(FacingDirection.Down);
        Stop();
        yield return new WaitForSeconds(0.5f);
        yield return Fader.FadeOut(1f);
    }

    public IEnumerator PlayEnding()
    {
        GameManager.Instance.PauseGame(true);
        yield return AudioManager.Instance.StopMusic(true);
        yield return Fader.FadeIn(2f);
        _videoPlayer.Stop();
        _videoPlayer.clip = _videoClips[(int)VIDEO.ENDING];
        _videoPlayer.isLooping = false;
        yield return new WaitForSeconds(2f);
        _videoTexture.gameObject.SetActive(true);
        _videoPlayer.Play();
        yield return Fader.FadeOut(1f);
        while (_videoPlayer.isPlaying)
        {
            yield return new WaitForFixedUpdate(); //跟FixedUpdate 一样根据固定帧 更新
        }
        yield return Fader.FadeIn(1f);
        _videoTexture.gameObject.SetActive(false);
        Stop();
        yield return new WaitForSeconds(1f);
        GameManager.Instance.GoToEnding();
    }

    public IEnumerator PlayVideo(VIDEO video)
    {
        _videoTexture.gameObject.SetActive(true);
        int index = (int)video;
        _videoPlayer.clip = _videoClips[index];
        _videoPlayer.isLooping = false;
        _videoPlayer.Play();
        while (_videoPlayer.isPlaying) 
        {
            yield return new WaitForFixedUpdate(); //跟FixedUpdate 一样根据固定帧 更新
        }
        yield return Fader.FadeIn(0.5f);
        _videoTexture.gameObject.SetActive(false);
    }

    public void PlayLoopVideo(VIDEO video)
    {
        _videoTexture.gameObject.SetActive(true);
        int index = (int)video;
        _videoPlayer.clip = _videoClips[index];
        _videoPlayer.Play();
        _videoPlayer.isLooping = true;
    }

    public void Stop()
    {
        _videoPlayer.Stop();
        _videoTexture.gameObject.SetActive(false);
    }

}

public enum VIDEO
{
    TITLE,
    OPENNING,
    ENDING,
    GAME_CLEAR
}
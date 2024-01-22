using PokeGenshinUtils.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionState : State<GameManager>, ISavable
{
    [SerializeField] private OptionMenuUI _optionMenuUI;

    public static OptionState I { get; private set; }
    public OptionMenuUI OptionMenuUI { get => _optionMenuUI; set => _optionMenuUI = value; }

    private GameManager _gameManager;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
        _optionMenuUI.OnSelected += OnItemSelected;
        _optionMenuUI.OnBack += OnBack;
        _optionMenuUI.BGMSlider.value = AudioManager.Instance.MusicPlayer.volume;
        _optionMenuUI.SFXSlider.value = AudioManager.Instance.SfxPlayer.volume;
        _optionMenuUI.Show();
    }

    public override void Execute()
    {
        _optionMenuUI.HandleUpdate();
    }

    public override void Exit(bool sfx = true)
    {
        _optionMenuUI.Close();
        _optionMenuUI.OnSelected -= OnItemSelected;
        _optionMenuUI.OnBack -= OnBack;
    }

    private void OnItemSelected(int selection)
    {
        
    }

    private void OnBack()
    {
        _gameManager.StateMachine.Pop();
    }

    public object CaptureState()
    {
        var saveData = new OptionSaveData()
        {
            BGMVolume = AudioManager.Instance.MusicPlayer.volume,
            SFXVolume = AudioManager.Instance.SfxPlayer.volume,
            IsDoubleSpeed = _optionMenuUI.IsDoubleSpeed
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (OptionSaveData)state;
        _optionMenuUI.BGMSlider.value = saveData.BGMVolume;
        _optionMenuUI.SFXSlider.value = saveData.SFXVolume;
        AudioManager.Instance.MusicPlayer.volume = saveData.BGMVolume;
        AudioManager.Instance.SfxPlayer.volume = saveData.SFXVolume;
        _optionMenuUI.IsDoubleSpeed = saveData.IsDoubleSpeed;
        if (_optionMenuUI.IsDoubleSpeed)
        {
            GameManager.Instance.ChangeTimeScale(2f);
        }
        else
        {
            GameManager.Instance.ChangeTimeScale(1f);
        }
    }
}

[Serializable]
public class OptionSaveData{
    public float BGMVolume;
    public float SFXVolume;
    public bool IsDoubleSpeed;
}

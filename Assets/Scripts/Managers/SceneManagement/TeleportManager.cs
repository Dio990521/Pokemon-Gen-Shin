﻿using Game.Tool.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IManager
{
    public void ResetData();

    public void Init();
}


public class TeleportManager : Singleton<TeleportManager>, ISavable, IManager
{
    [SerializeField] private List<TeleportData> _teleports;

    public List<TeleportData> Teleports { get {  return _teleports; } }

    public void ResetData()
    {
        foreach (var tele in _teleports)
        {
            tele.IsActive = false;
        }
    }

    public void Init()
    {

    }

    public List<string> GetActiveList()
    {
        List<string> teleNames = new List<string>();
        foreach (var tele in _teleports)
        {
            if (tele.IsActive)
            {
                teleNames.Add(tele.TeleportName);
            }
        }
        return teleNames;
    }

    public List<int> GetActiveTeleportIndex()
    {
        List<int> teleIndex = new List<int>();
        for (int i = 0; i < _teleports.Count; ++i)
        {
            if (_teleports[i].IsActive)
            {
                teleIndex.Add(i);
            }
        }
        return teleIndex;
    }

    public IEnumerator GameOverTransport(bool takeMoney=true)
    {
        yield return AudioManager.Instance.StopMusic(true);
        GameManager.Instance.PauseGame(true);
        var player = GameManager.Instance.PlayerController;
        var playerParty = player.GetComponent<PokemonParty>();
        if (GameKeyManager.Instance.GetBoolValue(CutsceneName.第一次来提瓦特.ToString()))
        {
            player.Character.SetPositionAndSnapToTile(new Vector2(-64.5f, 135.65f));
        }
        else
        {
            player.Character.SetPositionAndSnapToTile(new Vector2(-8.5f, -22.5f));
        }
        playerParty.Pokemons.ForEach(p => p.Heal());
        playerParty.PartyUpdated();
        if (takeMoney)
            Wallet.I.TakeMoneyPercentage(0.1f);
        player.Character.Animator.SetFacingDirection(FacingDirection.Down);
        

        yield return new WaitForSeconds(1f);
        GameManager.Instance.PauseGame(false);
    }

    public object CaptureState()
    {
        var saveData = new List<bool>();
        foreach (var tele in _teleports)
        {
            saveData.Add(tele.IsActive);
        }
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (List<bool>)state;
        for (int i = 0; i < saveData.Count; ++i)
        {
            _teleports[i].IsActive = saveData[i];
        }
    }
}

[Serializable]
public class TeleportData
{
    [SerializeField] private string _teleportName;
    [SerializeField] private bool _isActive;
    [SerializeField] private Vector2 _spawnPoint;

    public string TeleportName { get { return _teleportName; } }

    public bool IsActive { get { return _isActive; } set { _isActive = value; } }

    public Vector2 SpawnPoint { get {  return _spawnPoint; } }
}

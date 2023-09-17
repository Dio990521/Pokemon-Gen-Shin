using Game.Tool.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : Singleton<TeleportManager>, ISavable
{
    [SerializeField] private List<TeleportData> _teleports;

    public List<TeleportData> Teleports { get {  return _teleports; } }

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

    public IEnumerator GameOverTransport()
    {
        yield return AudioManager.Instance.StopMusic(true);
        GameManager.Instance.PauseGame(true);
        var player = GameManager.Instance.PlayerController;
        player.Character.SetPositionAndSnapToTile(new Vector2(-64.5f, 135.65f));
        player.Character.Animator.SetFacingDirection(FacingDirection.Down);
        var playerParty = player.GetComponent<PokemonParty>();
        playerParty.Pokemons.ForEach(p => p.Heal());
        playerParty.PartyUpdated();
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

using Game.Tool;
using Game.Tool.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameKeyManager : Singleton<GameKeyManager>, ISavable
{
    private Dictionary<string, int> _gameKeyIntDict;
    private Dictionary<string, bool> _gameKeyBoolDict;

    protected override void Awake()
    {
        base.Awake();
        _gameKeyIntDict = new Dictionary<string, int>();
        _gameKeyBoolDict = new Dictionary<string, bool>();
        foreach (var cutsceneName in System.Enum.GetValues(typeof(CutsceneName)))
        {
            AddKey(cutsceneName.ToString(), false);
        }
    }

    public void AddKey(string key, int value=0)
    {
        if (!_gameKeyIntDict.ContainsKey(key))
        {
            _gameKeyIntDict.Add(key, value);
        }
        else
        {
            DevelopmentToos.WTF($"there is a key named {key}");
        }
    }

    public void AddKey(string key, bool value = false)
    {
        if (!_gameKeyBoolDict.ContainsKey(key))
        {
            _gameKeyBoolDict.Add(key, value);
        }
        else
        {
            DevelopmentToos.WTF($"there is a key named {key}");
        }
    }

    public void SetIntValue(string key, int value)
    {
        if (_gameKeyIntDict.ContainsKey(key))
        {
            _gameKeyIntDict[key] = value;
        }
    }

    public void SetBoolValue(string key, bool value)
    {
        if (_gameKeyBoolDict.ContainsKey(key))
        {
            _gameKeyBoolDict[key] = value;
        }
    }

    public int GetIntValue(string key)
    {
        if (_gameKeyIntDict.ContainsKey(key))
        {
            return _gameKeyIntDict[key];
        }
        return -1;
    }

    public bool GetBoolValue(string key)
    {
        if (_gameKeyBoolDict.ContainsKey(key))
        {
            return _gameKeyBoolDict[key];
        }
        return false;
    }

    public object CaptureState()
    {
        var saveData = new GameKeySaveData()
        {
            GameKeyBoolDict = _gameKeyBoolDict,
            GameKeyIntDict = _gameKeyIntDict,
        };
        return saveData;
        
    }

    public void RestoreState(object state)
    {
        var saveData = (GameKeySaveData)state;
        _gameKeyBoolDict = saveData.GameKeyBoolDict;
        _gameKeyIntDict = saveData.GameKeyIntDict;
    }
}

[System.Serializable]
public class GameKeySaveData
{
    public Dictionary<string, int> GameKeyIntDict;
    public Dictionary<string, bool> GameKeyBoolDict;
}
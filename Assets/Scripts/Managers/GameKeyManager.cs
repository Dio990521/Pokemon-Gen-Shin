using Game.Tool;
using Game.Tool.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameKeyManager : Singleton<GameKeyManager>
{
    private Dictionary<string, int> _gameKeyDict;

    protected override void Awake()
    {
        base.Awake();
        _gameKeyDict = new Dictionary<string, int>();
    }

    public void AddKey(string key, int value=0)
    {
        if (!_gameKeyDict.ContainsKey(key))
        {
            _gameKeyDict.Add(key, value);
        }
        else
        {
            DevelopmentToos.WTF($"there is a key named {key}");
        }
    }

    public int GetValue(string key)
    {
        if (_gameKeyDict.ContainsKey(key))
        {
            return _gameKeyDict[key];
        }
        return -1;
    }

}

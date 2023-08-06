using System;
using System.Collections.Generic;

[Serializable]
public class SaveLoadData
{
    public List<string> playTimes;
    public List<string> achievements;
    public List<string> scenes;
    public List<string> dates;
    public List<bool> actives;

    public SaveLoadData()
    {
        playTimes = new List<string>() { null, null, null };
        achievements = new List<string>() { null, null, null };
        scenes = new List<string>() { null, null, null };
        dates = new List<string>() { null, null, null };
        actives = new List<bool>() { false, false, false };
    }
}

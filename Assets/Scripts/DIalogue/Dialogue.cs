using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [SerializeField] private string speakerName;
    [SerializeField] private List<string> lines;
    [SerializeField] private CutsceneName beforeCutscene = CutsceneName.None;

    public List<string> Lines
    {
        get { return lines; }
    }

    public CutsceneName InCutscene { get => beforeCutscene; set => beforeCutscene = value; }
    public string SpeakerName { get => speakerName; set => speakerName = value; }
}

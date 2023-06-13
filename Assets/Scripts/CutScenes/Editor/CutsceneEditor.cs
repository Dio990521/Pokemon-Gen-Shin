using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Cutscene))]
public class CutsceneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var cutscene = target as Cutscene;
        if (GUILayout.Button("Add Dialogue Action"))
        {
            cutscene.AddAction(new DialogueAction());
        }
        else if (GUILayout.Button("Add Move Actor Action"))
        {
            cutscene.AddAction(new MoveActorAction());
        }
        else if (GUILayout.Button("Add Turn Actor Action"))
        {
            cutscene.AddAction(new TurnActorAction());
        }
        else if (GUILayout.Button("Add Teleport Object Action"))
        {
            cutscene.AddAction(new TeleportObjectAction());
        }
        base.OnInspectorGUI();
    }
}

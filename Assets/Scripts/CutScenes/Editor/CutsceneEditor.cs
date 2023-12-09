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
        using (var scope = new GUILayout.HorizontalScope())
        {

            if (GUILayout.Button("Dialogue"))
            {
                cutscene.AddAction(new DialogueAction());
            }
            else if (GUILayout.Button("Move Actor"))
            {
                cutscene.AddAction(new MoveActorAction());
            }
            else if (GUILayout.Button("Turn Actor"))
            {
                cutscene.AddAction(new TurnActorAction());
            }
        }

        using (var scope = new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Teleport Object"))
            {
                cutscene.AddAction(new TeleportObjectAction());
            }
            else if (GUILayout.Button("Enable Object"))
            {
                cutscene.AddAction(new EnableObjectAction());
            }
            else if (GUILayout.Button("Disable Object"))
            {
                cutscene.AddAction(new DisableObjectAction());
            }
        }

        using (var scope = new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("NPC Interact"))
            {
                cutscene.AddAction(new NPCInteractAction());
            }
            else if (GUILayout.Button("Fade In"))
            {
                cutscene.AddAction(new FadeInAction());
            }
            else if (GUILayout.Button("Fade Out"))
            {
                cutscene.AddAction(new FadeOutAction());
            }
        }

        using (var scope = new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Teleport Player"))
            {
                cutscene.AddAction(new TeleportPlayerAction());
            }
            else if (GUILayout.Button("Switch BGM"))
            {
                cutscene.AddAction(new SwitchBGMAction());
            }
            else if (GUILayout.Button("Reload Connected Scene"))
            {
                cutscene.AddAction(new ReloadConnectedSceneAction());
            }
        }

        using (var scope = new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Wait"))
            {
                cutscene.AddAction(new WaitAction());
            }
            if (GUILayout.Button("Play SE"))
            {
                cutscene.AddAction(new PlaySEAction());
            }
            if (GUILayout.Button("Exclamation Effect"))
            {
                cutscene.AddAction(new ExclamationEffectAction());
            }

        }

        base.OnInspectorGUI();
    }
}

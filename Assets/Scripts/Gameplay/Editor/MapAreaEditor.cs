using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(MapArea))]
public class MapAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        int totalChanceInGrass = serializedObject.FindProperty("totalChance").intValue;
        int totalChanceInWater = serializedObject.FindProperty("totalChanceInWater").intValue;
        int totalChanceInDesert = serializedObject.FindProperty("totalChanceInDesert").intValue;
        int totalChanceInCave = serializedObject.FindProperty("totalChanceInCave").intValue;

        //var style = new GUIStyle();
        //style.fontStyle = FontStyle.Bold;

        //GUILayout.Label($"Total Chance = {totalChanceInGrass}", style);

        if (totalChanceInGrass != 100 && totalChanceInGrass != -1)
        {
            EditorGUILayout.HelpBox($"The total chance percentage in grass is {totalChanceInGrass}%, not 100%", MessageType.Error);
        }
        if (totalChanceInWater != 100 && totalChanceInWater != -1)
        {
            EditorGUILayout.HelpBox($"The total chance percentage in water is {totalChanceInWater}%, not 100%", MessageType.Error);
        }
        if (totalChanceInDesert != 100 && totalChanceInDesert != -1)
        {
            EditorGUILayout.HelpBox($"The total chance percentage in desert is {totalChanceInDesert}%, not 100%", MessageType.Error);
        }
        if (totalChanceInCave != 100 && totalChanceInCave != -1)
        {
            EditorGUILayout.HelpBox($"The total chance percentage in desert is {totalChanceInCave}%, not 100%", MessageType.Error);
        }
    }
}

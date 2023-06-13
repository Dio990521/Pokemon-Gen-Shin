using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CutsceneActor))]
public class CutsceneActorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, label);

        var togglePos = new Rect(position.x, position.y, 70, position.height);
        var fieldPos = new Rect(position.x + 70, position.y, position.width - 70, position.height);
        
        var isPlayerProp =  property.FindPropertyRelative("isPlayer");
        isPlayerProp.boolValue = GUI.Toggle(togglePos, isPlayerProp.boolValue, "Is Player");
        isPlayerProp.serializedObject.ApplyModifiedProperties();

        if (!isPlayerProp.boolValue)
        {
            EditorGUI.PropertyField(fieldPos, property.FindPropertyRelative("character"), GUIContent.none);
        }

        EditorGUI.EndProperty();
    }
}

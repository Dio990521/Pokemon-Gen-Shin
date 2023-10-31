using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class OpenSceneEditor : Editor
{
    [MenuItem("��չ/��Game Play����")]
    static void OpenMyScene()
    {
        string scenePath = "Assets/Scenes/GamePlay.unity";

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }

    [MenuItem("��չ/��δ���򳡾�")]
    static void OpenWeibaiTownScenes()
    {
        string folderPath = "Assets/Scenes/WeibaiTown";

        string[] sceneGUIDs = AssetDatabase.FindAssets("t:Scene", new[] { folderPath });

        if (sceneGUIDs != null)
        {
            foreach (string sceneGUID in sceneGUIDs)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
        }
    }

    [MenuItem("��չ/������ɭ�ֳ���")]
    static void OpenXumengForestScenes()
    {
        string folderPath = "Assets/Scenes/XumengForest";

        string[] sceneGUIDs = AssetDatabase.FindAssets("t:Scene", new[] { folderPath });

        if (sceneGUIDs != null)
        {
            foreach (string sceneGUID in sceneGUIDs)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
        }
    }

    [MenuItem("��չ/���ֵº�̲����")]
    static void OpenMideBeachScenes()
    {
        string folderPath = "Assets/Scenes/MideBeach";

        string[] sceneGUIDs = AssetDatabase.FindAssets("t:Scene", new[] { folderPath });

        if (sceneGUIDs != null)
        {
            foreach (string sceneGUID in sceneGUIDs)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
        }
    }

    [MenuItem("��չ/��С�����س���")]
    static void OpenTiwateScenes()
    {
        string folderPath = "Assets/Scenes/Tiwate";

        string[] sceneGUIDs = AssetDatabase.FindAssets("t:Scene", new[] { folderPath });

        if (sceneGUIDs != null)
        {
            foreach (string sceneGUID in sceneGUIDs)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
        }
    }

    [MenuItem("��չ/������ɽ����")]
    static void OpenNataMountainScenes()
    {
        string folderPath = "Assets/Scenes/NataMountain";

        string[] sceneGUIDs = AssetDatabase.FindAssets("t:Scene", new[] { folderPath });

        if (sceneGUIDs != null)
        {
            foreach (string sceneGUID in sceneGUIDs)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
        }
    }

    [MenuItem("��չ/������ɳĮ����")]
    static void OpenLidaoDesertScenes()
    {
        string folderPath = "Assets/Scenes/LidaoDesert";

        string[] sceneGUIDs = AssetDatabase.FindAssets("t:Scene", new[] { folderPath });

        if (sceneGUIDs != null)
        {
            foreach (string sceneGUID in sceneGUIDs)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
        }
    }

    [MenuItem("��չ/�򿪾���������")]
    static void OpenArenaScenes()
    {
        string folderPath = "Assets/Scenes/Arena";

        string[] sceneGUIDs = AssetDatabase.FindAssets("t:Scene", new[] { folderPath });

        if (sceneGUIDs != null)
        {
            foreach (string sceneGUID in sceneGUIDs)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
        }
    }

    [MenuItem("��չ/�����г���")]
    static void OpenAllScenes()
    {
        string[] sceneGUIDs = AssetDatabase.FindAssets("t:Scene");

        if (sceneGUIDs != null)
        {
            foreach (string sceneGUID in sceneGUIDs)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
        }
    }
}

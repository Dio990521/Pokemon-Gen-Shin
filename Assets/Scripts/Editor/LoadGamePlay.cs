using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class OpenSceneEditor : Editor
{
    [MenuItem("拓展/打开Game Play场景")]
    static void OpenMyScene()
    {
        string scenePath = "Assets/Scenes/GamePlay.unity";

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }

    [MenuItem("拓展/打开未白镇场景")]
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

    [MenuItem("拓展/打开须蒙森林场景")]
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

    [MenuItem("拓展/打开弥德海滩场景")]
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

    [MenuItem("拓展/打开小提瓦特场景")]
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

    [MenuItem("拓展/打开纳塔山场景")]
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

    [MenuItem("拓展/打开璃稻沙漠场景")]
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

    [MenuItem("拓展/打开竞技场场景")]
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

    [MenuItem("拓展/打开所有场景")]
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

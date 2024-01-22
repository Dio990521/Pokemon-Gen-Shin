using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
//using UnityEngine.AddressableAssets;

public class ScriptableObjectDB<T> : MonoBehaviour where T : ScriptableObject
{
    protected static Dictionary<string, T> objects;
    
    public static void Init()
    {
        objects = new Dictionary<string, T>();
        string className = typeof(T).Name;
        Debug.Log($"{className}开始加载数据");
        StringBuilder loadPath = new StringBuilder();
        loadPath.Append("ScriptableObjects/");
        loadPath.Append(className.Substring(0, className.Length - 4));
        loadPath.Append("s");
        //AssetLabelReference assetLabelReference = new();
        //assetLabelReference.labelString = loadPath.ToString();
        //Addressables.LoadAssetsAsync<T>(assetLabelReference, (obj) =>
        //{
        //    if (objects.ContainsKey(obj.name))
        //    {
        //        Debug.LogError($"There are 2 objects with the same name: {obj.name}");
        //    }
        //    else
        //    {
        //        objects[obj.name] = obj;
        //    }
        //}).Completed += (handle) =>
        //{
        //    Debug.Log($"{className}初始化完成！" + " 已加载数据量： " + objects?.Count);
        //};
        var objectArray = Resources.LoadAll<T>(loadPath.ToString());
        foreach (var obj in objectArray)
        {
            if (objects.ContainsKey(obj.name))
            {
                Debug.LogError($"There are 2 objects with the same name: {obj.name}");
                continue;
            }
            objects[obj.name] = obj;
        }
    }

    public static T GetObjectByName(string name)
    {
        if (!objects.ContainsKey(name))
        {
            Debug.LogError($"Object not found with the name {name}");
            return null;
        }
        return objects[name];
    }

    public static ICollection<string> GetAllKeys()
    {
        return objects.Keys;
    }

    public static string GetRandomKey()
    {
        int randomIndex = UnityEngine.Random.Range(0, objects.Count);
        int currentIndex = 0;
        foreach (var key in objects.Keys)
        {
            if (currentIndex == randomIndex)
            {
                return key;
            }
            currentIndex++;
        }
        throw new InvalidOperationException("字典为空或索引越界。");
    }


}


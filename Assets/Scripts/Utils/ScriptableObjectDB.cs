using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ScriptableObjectDB<T> : MonoBehaviour where T : ScriptableObject
{
    private static Dictionary<string, T> objects;
    
    public static void Init()
    {
        objects = new Dictionary<string, T>();
        string className = typeof(T).Name;
        StringBuilder loadPath = new StringBuilder();
        loadPath.Append("Prefabs/");
        loadPath.Append(className.Substring(0, className.Length - 4));
        loadPath.Append("s");
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


}


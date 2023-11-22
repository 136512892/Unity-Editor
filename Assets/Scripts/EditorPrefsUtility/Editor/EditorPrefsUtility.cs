using UnityEngine;
using UnityEditor;

public class EditorPrefsUtility 
{
    public static void SetObject<T>(string key, T t)
    {
        string json = JsonUtility.ToJson(t);
        EditorPrefs.SetString(key, json);
    }

    public static T GetObject<T>(string key)
    {
        if (EditorPrefs.HasKey(key))
        {
            string json = EditorPrefs.GetString(key);
            return JsonUtility.FromJson<T>(json);
        }
        return default;
    }
}
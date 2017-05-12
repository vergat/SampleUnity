using UnityEngine;
using UnityEditor;

using System.IO;

public static class ScriptableObjectUtility
{
    public static void CreateAsset<T>(string i_AssetName = "") where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets/";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathName = "";

        if (i_AssetName == "")
        {
            assetPathName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");
        }
        else
        {
            assetPathName = AssetDatabase.GenerateUniqueAssetPath(path + i_AssetName + ".asset");
        }

        AssetDatabase.CreateAsset(asset, assetPathName);

        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
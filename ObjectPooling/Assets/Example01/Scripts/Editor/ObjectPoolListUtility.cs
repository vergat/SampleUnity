using UnityEngine;
using UnityEditor;
using System.Collections;

public class ObjectPoolListUtility
{
    [MenuItem("Assets/Create/ObjectPool/List")]
    public static void CreateItem()
    {
        ScriptableObjectUtility.CreateAsset<ObjectPoolList>();
    }
}

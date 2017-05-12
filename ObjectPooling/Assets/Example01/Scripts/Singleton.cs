using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif 

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _Instance                  = null;
    private static bool _bApplicationIsQuitting = false;

    public static T Instance
    {
        get
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                return null;
            }
#endif 

            if (_bApplicationIsQuitting)
            {
                return null;
            }

            if (_Instance == null)
            {
                _Instance = GameObject.FindObjectOfType<T>();

                if (_Instance == null)
                {
                    // Create a new GameObject with DontDestroyOnLoad.

                    GameObject go = new GameObject();
                    go.name = typeof(T).Name.ToString();

                    DontDestroyOnLoad(go);

                    // Add component.

                    _Instance = go.AddComponent<T>();
                }
            }

            return _Instance;
        }
    }

    public void OnDestroy()
    {
        OnSingletonDestroy();

        _bApplicationIsQuitting = true;
    }

    // VIRTUAL

    protected virtual void OnSingletonDestroy()
    {

    }
}
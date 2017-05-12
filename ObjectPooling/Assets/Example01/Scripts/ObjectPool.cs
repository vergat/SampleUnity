using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class ObjectPool : Singleton<ObjectPool>
{
    private List<GameObject> m_TempList = new List<GameObject>();

    private Dictionary<GameObject, List<GameObject>> m_PooledObjects = new Dictionary<GameObject, List<GameObject>>();
    private Dictionary<GameObject, List<GameObject>> m_Consumables = new Dictionary<GameObject, List<GameObject>>();

    private Dictionary<GameObject, GameObject> m_SpawnedObjects = new Dictionary<GameObject, GameObject>();

    private List<GameObject> m_ToDestroy = new List<GameObject>();

    // MonoBehaviour's INTERFACE

    void Awake()
    {

    }

    // BUSINESS LOGIC

    public void Initialize()
    {

    }

    // Create Pool

    public void CreatePool(GameObject i_Prefab, int i_InitialPoolSize, bool i_AllowRecycle)
    {
        if (i_Prefab == null || i_InitialPoolSize <= 0)
            return;

        if (m_PooledObjects.ContainsKey(i_Prefab))
            return;

        if (m_Consumables.ContainsKey(i_Prefab))
            return; 

        List<GameObject> list = new List<GameObject>();

        if (i_AllowRecycle)
        {
            m_PooledObjects.Add(i_Prefab, list);
        }
        else
        {
            m_Consumables.Add(i_Prefab, list);
        }

        bool active = i_Prefab.activeSelf;
        i_Prefab.SetActive(false);

        while (list.Count < i_InitialPoolSize)
        {
            var obj = (GameObject)Object.Instantiate(i_Prefab);
            obj.transform.SetParent(transform);
            list.Add(obj);
        }

        i_Prefab.SetActive(active);
    }

    public void CreatePool<T>(T i_Prefab, int i_InitialPoolSize, bool i_AllowRecycle) where T : Component
    {
        CreatePool(i_Prefab.gameObject, i_InitialPoolSize, i_AllowRecycle);
    }

    // Spawn

    public GameObject Spawn(GameObject i_Prefab, Transform i_Parent, Vector3 i_Position, Quaternion i_Rotation)
    {
        Transform trans;
        GameObject obj;

        List<GameObject> list;
        if (TryGetPool(i_Prefab, out list))
        {
            obj = null;
            if (list.Count > 0)
            {
                while (obj == null && list.Count > 0)
                {
                    obj = list[0];
                    list.RemoveAt(0);
                }

                if (obj != null)
                {
                    trans = obj.transform;
                    trans.SetParent(i_Parent);
                    trans.localPosition = i_Position;
                    trans.localRotation = i_Rotation;
                    obj.SetActive(true);

                    m_SpawnedObjects.Add(obj, i_Prefab);

                    return obj;
                }
            }

            obj = (GameObject)Object.Instantiate(i_Prefab);
            trans = obj.transform;
            trans.SetParent(i_Parent);
            trans.localPosition = i_Position;
            trans.localRotation = i_Rotation;

            m_SpawnedObjects.Add(obj, i_Prefab);

            return obj;
        }
        else
        {
            obj = (GameObject)Object.Instantiate(i_Prefab);
            trans = obj.transform;
            trans.SetParent(i_Parent);
            trans.localPosition = i_Position;
            trans.localRotation = i_Rotation;
            return obj;
        }
    }

    public GameObject Spawn(GameObject i_Prefab, Transform i_Parent, Vector3 i_Position)
    {
        return Spawn(i_Prefab, i_Parent, i_Position, Quaternion.identity);
    }

    public GameObject Spawn(GameObject i_Prefab, Vector3 i_Position, Quaternion i_Rotation)
    {
        return Spawn(i_Prefab, null, i_Position, i_Rotation);
    }

    public GameObject Spawn(GameObject i_Prefab, Transform i_Parent)
    {
        return Spawn(i_Prefab, i_Parent, Vector3.zero, Quaternion.identity);
    }

    public GameObject Spawn(GameObject i_Prefab, Vector3 i_Position)
    {
        return Spawn(i_Prefab, null, i_Position, Quaternion.identity);
    }

    public GameObject Spawn(GameObject i_Prefab)
    {
        return Spawn(i_Prefab, null, Vector3.zero, Quaternion.identity);
    }

    public T Spawn<T>(T i_Prefab, Transform i_Parent, Vector3 i_Position, Quaternion i_Rotation) where T : Component
    {
        return Spawn(i_Prefab.gameObject, i_Parent, i_Position, i_Rotation).GetComponent<T>();
    }

    public T Spawn<T>(T i_Prefab, Vector3 i_Position, Quaternion i_Rotation) where T : Component
    {
        return Spawn(i_Prefab.gameObject, null, i_Position, i_Rotation).GetComponent<T>();
    }

    public T Spawn<T>(T i_Prefab, Transform i_Parent, Vector3 i_Position) where T : Component
    {
        return Spawn(i_Prefab.gameObject, i_Parent, i_Position, Quaternion.identity).GetComponent<T>();
    }

    public T Spawn<T>(T i_Prefab, Vector3 i_Position) where T : Component
    {
        return Spawn(i_Prefab.gameObject, null, i_Position, Quaternion.identity).GetComponent<T>();
    }

    public T Spawn<T>(T i_Prefab, Transform i_Parent) where T : Component
    {
        return Spawn(i_Prefab.gameObject, i_Parent, Vector3.zero, Quaternion.identity).GetComponent<T>();
    }

    public T Spawn<T>(T i_Prefab) where T : Component
    {
        return Spawn(i_Prefab.gameObject, null, Vector3.zero, Quaternion.identity).GetComponent<T>();
    }

    // Recycle

    public void Recycle(GameObject i_Obj)
    {
        GameObject prefab;

        if (m_SpawnedObjects.TryGetValue(i_Obj, out prefab))
        {
            if (m_PooledObjects.ContainsKey(prefab))
            {
                m_PooledObjects[prefab].Add(i_Obj);
            }
            else
            {
                m_ToDestroy.Add(i_Obj);
            }

            i_Obj.transform.SetParent(transform);
            i_Obj.SetActive(false);

            m_SpawnedObjects.Remove(i_Obj);
        }
        else
        {
            Object.Destroy(i_Obj);
        }
    }

    public void Recycle<T>(T i_Obj) where T : Component
    {
        Recycle(i_Obj.gameObject);
    }

    public void RecycleAll(GameObject i_Prefab)
    {
        foreach (var item in m_SpawnedObjects)
        {
            if (item.Value == i_Prefab)
            {
                m_TempList.Add(item.Key);
            }
        }

        for (int i = 0; i < m_TempList.Count; ++i)
        {
            Recycle(m_TempList[i]);
        }

        m_TempList.Clear();
    }

    public void RecycleAll<T>(T i_Prefab) where T : Component
    {
        RecycleAll(i_Prefab.gameObject);
    }

    public void RecycleAll()
    {
        m_TempList.AddRange(m_SpawnedObjects.Keys);
        for (int i = 0; i < m_TempList.Count; ++i)
        {
            Recycle(m_TempList[i]);
        }

        m_TempList.Clear();
    }

    // Extra

    public bool IsSpawned(GameObject i_Obj)
    {
        return m_SpawnedObjects.ContainsKey(i_Obj);
    }

    public int CountPooled(GameObject i_Prefab)
    {
        List<GameObject> list;
        if (TryGetPool(i_Prefab, out list))
        {
            return list.Count;
        }

        return 0;
    }

    public int CountPooled<T>(T i_Prefab) where T : Component
    {
        return CountPooled(i_Prefab.gameObject);
    }

    public int CountSpawned(GameObject i_Prefab)
    {
        int count = 0;
        foreach (var instancePrefab in m_SpawnedObjects.Values)
        {
            if (i_Prefab == instancePrefab)
            {
                ++count;
            }
        }

        return count;
    }

    public int CountSpawned<T>(T i_Prefab) where T : Component
    {
        return CountSpawned(i_Prefab.gameObject);
    }

    public int CountAllPooled()
    {
        int count = 0;

        foreach (var list in m_PooledObjects.Values)
        {
            count += list.Count;
        }

        foreach (var list in m_Consumables.Values)
        {
            count += list.Count;
        }

        return count;
    }

    // Destroy

    public void DestroyPooled(GameObject i_Prefab)
    {
        List<GameObject> pooled;
        if (TryGetPool(i_Prefab, out pooled))
        {
            for (int i = 0; i < pooled.Count; ++i)
            {
                GameObject.Destroy(pooled[i]);
            }

            pooled.Clear();
        }
    }

    public void DestroyPooled<T>(T i_Prefab) where T : Component
    {
        DestroyPooled(i_Prefab.gameObject);
    }

    public void DestroyAll(GameObject i_Prefab)
    {
        RecycleAll(i_Prefab);
        DestroyPooled(i_Prefab);
    }

    public void DestroyAll<T>(T i_Prefab) where T : Component
    {
        DestroyAll(i_Prefab.gameObject);
    }

    // Release

    public void ReleaseSpawned()
    {
        m_SpawnedObjects.Clear();

        for (int index = 0; index < m_ToDestroy.Count; ++index)
        {
            GameObject goInstance = m_ToDestroy[index];
            GameObject.Destroy(goInstance);
        }

        m_ToDestroy.Clear();
    }

    // INTERNALS

    private bool TryGetPool(GameObject i_Prefab, out List<GameObject> o_Pool)
    {
        if (i_Prefab == null)
        {
            o_Pool = null;
            return false;
        }

        if (!m_PooledObjects.TryGetValue(i_Prefab, out o_Pool))
        {
            if (!m_Consumables.TryGetValue(i_Prefab, out o_Pool))
            {
                return false;
            }
        }

        return true;
    }

    // STATIC METHODS
    
    public static void InitializeMain()
    {
        if (Instance != null)
        {
            Instance.Initialize();
        }
    }

    public static void CreatePoolMain(GameObject i_Prefab, int i_InitialPoolSize, bool i_AllowRecycle)
    {
        if (Instance != null)
        {
            Instance.CreatePool(i_Prefab, i_InitialPoolSize, i_AllowRecycle);
        }
    }

    public static void CreatePoolMain<T>(T i_Prefab, int i_InitialPoolSize, bool i_AllowRecycle) where T : Component
    {
        if (Instance != null)
        {
            Instance.CreatePool(i_Prefab, i_InitialPoolSize, i_AllowRecycle);
        }
    }

    public static GameObject SpawnMain(GameObject i_Prefab, Transform i_Parent, Vector3 i_Position, Quaternion i_Rotation)
    {
        if (Instance != null)
        {
            return Instance.Spawn(i_Prefab, i_Parent, i_Position, i_Rotation);
        }

        return null;
    }

    public static GameObject SpawnMain(GameObject i_Prefab, Transform i_Parent, Vector3 i_Position)
    {
        if (Instance != null)
        {
            return Instance.Spawn(i_Prefab, i_Parent, i_Position, Quaternion.identity);
        }

        return null;
    }

    public static GameObject SpawnMain(GameObject i_Prefab, Vector3 i_Position, Quaternion i_Rotation)
    {
        if (Instance != null)
        {
            return Instance.Spawn(i_Prefab, null, i_Position, i_Rotation);
        }

        return null;
    }

    public static GameObject SpawnMain(GameObject i_Prefab, Transform i_Parent)
    {
        if (Instance != null)
        {
            return Instance.Spawn(i_Prefab, i_Parent, Vector3.zero, Quaternion.identity);
        }

        return null;
    }

    public static GameObject SpawnMain(GameObject i_Prefab, Vector3 i_Position)
    {
        if (Instance != null)
        {
           return Instance.Spawn(i_Prefab, null, i_Position, Quaternion.identity);
        }

        return null;
    }

    public static GameObject SpawnMain(GameObject i_Prefab)
    {
        if (Instance != null)
        {
            return Instance.Spawn(i_Prefab, null, Vector3.zero, Quaternion.identity);
        }

        return null;
    }

    public static T SpawnMain<T>(T i_Prefab, Transform i_Parent, Vector3 i_Position, Quaternion i_Rotation) where T : Component
    {
        if (Instance != null)
        {
            return Instance.Spawn(i_Prefab, i_Parent, i_Position, i_Rotation);
        }

        return null;
    }

    public static T SpawnMain<T>(T i_Prefab, Vector3 i_Position, Quaternion i_Rotation) where T : Component
    {
        if (Instance != null)
        {
            return Instance.Spawn(i_Prefab, null, i_Position, i_Rotation);
        }

        return null;
    }

    public static T SpawnMain<T>(T i_Prefab, Transform i_Parent, Vector3 i_Position) where T : Component
    {
        if (Instance != null)
        {
            return Instance.Spawn(i_Prefab, i_Parent, i_Position, Quaternion.identity);
        }

        return null;
    }

    public static T SpawnMain<T>(T i_Prefab, Vector3 i_Position) where T : Component
    {
        if (Instance != null)
        {
            return Instance.Spawn(i_Prefab, null, i_Position, Quaternion.identity);
        }

        return null;
    }

    public static T SpawnMain<T>(T i_Prefab, Transform i_Parent) where T : Component
    {
        if (Instance != null)
        {
            return Instance.Spawn(i_Prefab, i_Parent, Vector3.zero, Quaternion.identity);
        }

        return null;
    }

    public static T SpawnMain<T>(T i_Prefab) where T : Component
    {
        if (Instance != null)
        {
            return Instance.Spawn(i_Prefab, null, Vector3.zero, Quaternion.identity);
        }

        return null;
    }

    public static void RecycleMain(GameObject i_Obj)
    {
        if (Instance != null)
        {
            Instance.Recycle(i_Obj);
        }
    }

    public static void RecycleMain<T>(T i_Obj) where T : Component
    {
        if (Instance != null)
        {
            Instance.Recycle(i_Obj);
        }
    }

    public static void RecycleAllMain(GameObject i_Prefab)
    {
        if (Instance != null)
        {
            Instance.RecycleAll(i_Prefab);
        }
    }

    public static void RecycleAllMain<T>(T i_Prefab) where T : Component
    {
        if (Instance != null)
        {
            Instance.RecycleAll(i_Prefab);
        }
    }

    public static void RecycleAllMain()
    {
        if (Instance != null)
        {
            Instance.RecycleAll();
        }
    }

    public static bool IsSpawnedMain(GameObject i_Obj)
    {
        if (Instance != null)
        {
            return Instance.IsSpawned(i_Obj);
        }

        return false;
    }

    public static int CountPooledMain(GameObject i_Prefab)
    {
        if (Instance != null)
        {
            return Instance.CountPooled(i_Prefab);
        }

        return 0;
    }

    public static int CountPooledMain<T>(T i_Prefab) where T : Component
    {
        if (Instance != null)
        {
            return Instance.CountPooled(i_Prefab);
        }

        return 0;
    }

    public static int CountSpawnedMain(GameObject i_Prefab)
    {
        if (Instance != null)
        {
            return Instance.CountSpawned(i_Prefab);
        }

        return 0;
    }

    public static int CountSpawnedMain<T>(T i_Prefab) where T : Component
    {
        if (Instance != null)
        {
            return Instance.CountSpawned(i_Prefab);
        }

        return 0;
    }

    public static int CountAllPooledMain()
    {
        if (Instance != null)
        {
            return Instance.CountAllPooled();
        }

        return 0;
    }

    public static void DestroyPooledMain(GameObject i_Prefab)
    {
        if (Instance != null)
        {
            Instance.DestroyPooled(i_Prefab);
        }
    }

    public static void DestroyPooledMain<T>(T i_Prefab) where T : Component
    {
        if (Instance != null)
        {
            Instance.DestroyPooled(i_Prefab.gameObject);
        }
    }

    public static void DestroyAllMain(GameObject i_Prefab)
    {
        if (Instance != null)
        {
            Instance.DestroyAll(i_Prefab);
        }
    }

    public static void DestroyAllMain<T>(T i_Prefab) where T : Component
    {
        if (Instance != null)
        {
            Instance.DestroyAll(i_Prefab.gameObject);
        }
    }

    public static void ReleaseSpawnedMain()
    {
        if (Instance != null)
        {
            Instance.ReleaseSpawned();
        }
    }
}

public static class ObjectPoolExtensions
{
    public static void CreatePool(this GameObject i_Prefab, int i_InitialPoolSize, bool i_AllowRecycle)
    {
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.CreatePool(i_Prefab, i_InitialPoolSize, i_AllowRecycle);
        }
    }

    public static void CreatePool<T>(this T i_Prefab, int i_InitialPoolSize, bool i_AllowRecycle) where T : Component
    {
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.CreatePool(i_Prefab, i_InitialPoolSize, i_AllowRecycle);
        }
    }

    public static GameObject Spawn(this GameObject i_Prefab, Transform i_Parent, Vector3 i_Position, Quaternion i_Rotation)
    {
        if (ObjectPool.Instance != null)
        {
            return ObjectPool.Instance.Spawn(i_Prefab, i_Parent, i_Position, i_Rotation);
        }

        return null;
    }

    public static GameObject Spawn(this GameObject i_Prefab, Transform i_Parent, Vector3 i_Position)
    {
        if (ObjectPool.Instance != null)
        {
            return ObjectPool.Instance.Spawn(i_Prefab, i_Parent, i_Position, Quaternion.identity);
        }

        return null;
    }

    public static GameObject Spawn(this GameObject i_Prefab, Vector3 i_Position, Quaternion i_Rotation)
    {
        if (ObjectPool.Instance != null)
        {
            return ObjectPool.Instance.Spawn(i_Prefab, null, i_Position, i_Rotation);
        }

        return null;
    }

    public static GameObject Spawn(this GameObject i_Prefab, Transform i_Parent)
    {
        if (ObjectPool.Instance != null)
        {
            return ObjectPool.Instance.Spawn(i_Prefab, i_Parent, Vector3.zero, Quaternion.identity);
        }

        return null;
    }

    public static GameObject Spawn(this GameObject i_Prefab, Vector3 i_Position)
    {
        if (ObjectPool.Instance != null)
        {
            return ObjectPool.Instance.Spawn(i_Prefab, null, i_Position, Quaternion.identity);
        }

        return null;
    }

    public static GameObject Spawn(this GameObject i_Prefab)
    {
        if (ObjectPool.Instance != null)
        {
            return ObjectPool.Instance.Spawn(i_Prefab, null, Vector3.zero, Quaternion.identity);
        }

        return null;
    }

    public static T Spawn<T>(this T i_Prefab, Transform i_Parent, Vector3 position, Quaternion i_Rotation) where T : Component
    {
        if (ObjectPool.Instance != null)
        {
            return ObjectPool.Instance.Spawn(i_Prefab, i_Parent, position, i_Rotation);
        }

        return null;
    }

    public static T Spawn<T>(this T i_Prefab, Vector3 i_Position, Quaternion i_Rotation) where T : Component
    {
        if (ObjectPool.Instance != null)
        {
            return ObjectPool.Instance.Spawn(i_Prefab, null, i_Position, i_Rotation);
        }

        return null;
    }

    public static T Spawn<T>(this T i_Prefab, Transform i_Parent, Vector3 i_Position) where T : Component
    {
        if (ObjectPool.Instance != null)
        {
            return ObjectPool.Instance.Spawn(i_Prefab, i_Parent, i_Position, Quaternion.identity);
        }

        return null;
    }

    public static T Spawn<T>(this T i_Prefab, Vector3 i_Position) where T : Component
    {
        if (ObjectPool.Instance != null)
        {
            return ObjectPool.Instance.Spawn(i_Prefab, null, i_Position, Quaternion.identity);
        }

        return null;
    }

    public static T Spawn<T>(this T i_Prefab, Transform i_Parent) where T : Component
    {
        if (ObjectPool.Instance != null)
        {
            return ObjectPool.Instance.Spawn(i_Prefab, i_Parent, Vector3.zero, Quaternion.identity);
        }

        return null;
    }

    public static T Spawn<T>(this T i_Prefab) where T : Component
    {
        if (ObjectPool.Instance != null)
        {
            return ObjectPool.Instance.Spawn(i_Prefab, null, Vector3.zero, Quaternion.identity);
        }

        return null;
    }

    public static void Recycle(this GameObject i_Obj)
    {
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.Recycle(i_Obj);
        }
    }

    public static void Recycle<T>(this T i_Obj) where T : Component
    {
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.Recycle(i_Obj);
        }
    }

    public static void RecycleAll(this GameObject i_Prefab)
    {
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.RecycleAll(i_Prefab);
        }
    }

    public static void RecycleAll<T>(this T i_Prefab) where T : Component
    {
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.RecycleAll(i_Prefab);
        }
    }

    public static void RecycleAll()
    {
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.RecycleAll();
        }
    }

    public static bool IsSpawned(this GameObject i_Obj)
    {
        if (ObjectPool.Instance != null)
        {
            return ObjectPool.Instance.IsSpawned(i_Obj);
        }

        return false;
    }

    public static int CountPooled(this GameObject i_Prefab)
    {
        if (ObjectPool.Instance != null)
        {
            return ObjectPool.Instance.CountPooled(i_Prefab);
        }

        return 0;
    }

    public static int CountPooled<T>(this T i_Prefab) where T : Component
    {
        if (ObjectPool.Instance != null)
        {
            return ObjectPool.Instance.CountPooled(i_Prefab);
        }

        return 0;
    }

    public static int CountSpawned(this GameObject i_Prefab)
    {
        if (ObjectPool.Instance != null)
        {
            return ObjectPool.Instance.CountSpawned(i_Prefab);
        }

        return 0;
    }

    public static int CountSpawned<T>(this T i_Prefab) where T : Component
    {
        if (ObjectPool.Instance != null)
        {
            return ObjectPool.Instance.CountSpawned(i_Prefab);
        }

        return 0;
    }

    public static void DestroyPooled(this GameObject i_Prefab)
    {
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.DestroyPooled(i_Prefab);
        }
    }

    public static void DestroyPooled<T>(this T i_Prefab) where T : Component
    {
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.DestroyPooled(i_Prefab.gameObject);
        }
    }

    public static void DestroyAll(this GameObject i_Prefab)
    {
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.DestroyAll(i_Prefab);
        }
    }

    public static void DestroyAll<T>(this T i_Prefab) where T : Component
    {
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.DestroyAll(i_Prefab.gameObject);
        }
    }
}

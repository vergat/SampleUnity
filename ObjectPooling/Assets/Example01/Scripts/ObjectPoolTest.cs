using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPoolTest : MonoBehaviour
{
    public GameObject prefab1 = null;
    public GameObject prefab2 = null;

    void Start ()
    {
        if (prefab1 != null)
        {
            int instances1 = 20;

            Debug.Log("[POOL TEST] Creating " + instances1 + " instances of " + prefab1.name);
            ObjectPool.CreatePoolMain(prefab1, instances1, true);

            Debug.Log("[POOL TEST] Test 1");
            Test1(prefab1, instances1);
        }

        if (prefab2 != null)
        {
            int instances2 = 20;

            Debug.Log("[POOL TEST] Creating " + instances2 + " instances of " + prefab2.name);
            ObjectPool.CreatePoolMain(prefab2, instances2, false);

            Debug.Log("[POOL TEST] Test 2");
            Test2(prefab2, instances2);
        }
    }

    void Test1(GameObject i_Prefab, int i_Size)
    {
        Check(ObjectPool.CountPooledMain(i_Prefab) == i_Size);

        for (int index = 1; index < i_Size; ++index)
        {
            List<GameObject> goCache = new List<GameObject>();

            for (int count = 0; count < index; ++count)
            {
                GameObject go = i_Prefab.Spawn();
                goCache.Add(go);
            }

            {
                int currentSize = ObjectPool.CountPooledMain(i_Prefab);
                Check(currentSize == (i_Size - index));
            }

            foreach (GameObject go in goCache)
            {
                go.Recycle();
            }

            {
                int currentSize = ObjectPool.CountPooledMain(i_Prefab);
                Check(currentSize == (i_Size));
            }
        }

        List<GameObject> cache = new List<GameObject>();

        for (int index = 0; index < 2 * i_Size; ++index)
        {
            GameObject go = i_Prefab.Spawn();
            cache.Add(go);
        }

        {
            int currentSize = ObjectPool.CountPooledMain(i_Prefab);
            Check(currentSize == 0);
        }

        foreach (GameObject go in cache)
        {
            go.Recycle();
        }

        {
            int currentSize = ObjectPool.CountPooledMain(i_Prefab);
            Check(currentSize == 2 * i_Size);
        }
    }

    void Test2(GameObject i_Prefab, int i_Size)
    {
        Check(ObjectPool.CountPooledMain(i_Prefab) == i_Size);

        List<GameObject> cache = new List<GameObject>();

        for (int index = 0; index < 2 * i_Size; ++index)
        {
            GameObject go = i_Prefab.Spawn();
            cache.Add(go);
        }

        // Check if size == 0.

        {
            int currentSize = ObjectPool.CountPooledMain(i_Prefab);
            Check(currentSize == 0);
        }

        // Check if all objects have been spawned.

        {
            int currentSize = ObjectPool.CountSpawnedMain(i_Prefab);
            Check(currentSize == 2 * i_Size);
        }

        foreach (GameObject go in cache)
        {
            go.Recycle();
        }

        // Check if size == 0.

        {
            int currentSize = ObjectPool.CountPooledMain(i_Prefab);
            Check(currentSize == 0);
        }
    }

    void Check(bool i_Value)
    {
        if (i_Value)
        {
            Debug.Log("OK");
        }
        else
        {
            Debug.LogError("FAILED");
        }
    }
}

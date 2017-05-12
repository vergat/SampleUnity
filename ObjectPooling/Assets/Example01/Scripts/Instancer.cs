using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

using Debug = UnityEngine.Debug;

public class Instancer : MonoBehaviour
{
    // Serializable fields

    [Header("Input")]

    [SerializeField]
    private KeyCode m_NoPoolTestKey = KeyCode.P;
    [SerializeField]
    private KeyCode m_PoolTestKey = KeyCode.L;

    [Header("Logic")]

    [SerializeField]
    private GameObject m_Prefab = null;

    [SerializeField]
    private int m_NumberOfInstances = 100;

    [SerializeField]
    private float m_SpawnPositionOffset = 0f;

    // Fields

    private List<GameObject> m_NoPoolObjects = new List<GameObject>();
    private List<GameObject> m_PoolObjects = new List<GameObject>();

    // MonoBehaviour's interface

    private void Update()
    {
        bool noPoolTestKeyDown = Input.GetKeyDown(m_NoPoolTestKey);
        bool poolTestKeyDown = Input.GetKeyDown(m_PoolTestKey);

        if (noPoolTestKeyDown != poolTestKeyDown)
        {
            if (noPoolTestKeyDown)
            {
                NoPoolTest();
            }

            if (poolTestKeyDown)
            {
                PoolTestKey();
            }
        }
    }

    // INTERNALS

    private void NoPoolTest()
    {
        StartCoroutine(RunNoPoolTest());
    }

    private void PoolTestKey()
    {
        StartCoroutine(RunPoolTest());
    }

    // COROUTINES

    private IEnumerator RunNoPoolTest()
    {
        Debug.Log("[No Pool Test] Start.");

        {
            Debug.Log("[No Pool Test] Instancing...");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int index = 0; index < m_NumberOfInstances; ++index)
            {
                GameObject go = Instantiate<GameObject>(m_Prefab);

                go.transform.position = Random.insideUnitSphere * m_SpawnPositionOffset;

                m_NoPoolObjects.Add(go);
            }

            sw.Stop();

            Debug.Log("[No Pool Test] Objects instantiated.");

            Debug.Log("[No Pool Test] Stopwatch: " + sw.ElapsedMilliseconds);
        }

        yield return new WaitForSeconds(2f);

        {
            Debug.Log("[No Pool Test] Destroying...");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int index = 0; index < m_NoPoolObjects.Count; ++index)
            {
                GameObject go = m_NoPoolObjects[index];
                Destroy(go);
            }

            m_NoPoolObjects.Clear();

            Debug.Log("[No Pool Test] Objects destroyed.");

            Debug.Log("[No Pool Test] Stopwatch: " + sw.ElapsedMilliseconds);
        }

        Debug.Log("[No Pool Test] End.");
    }

    private IEnumerator RunPoolTest()
    {
        Debug.Log("[Pool Test] Start.");

        {
            Debug.Log("[Pool Test] Instancing...");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int index = 0; index < m_NumberOfInstances; ++index)
            {
                GameObject go = m_Prefab.Spawn();

                go.transform.position = Random.insideUnitSphere * m_SpawnPositionOffset;

                m_PoolObjects.Add(go);
            }

            Debug.Log("[Pool Test] Objects instantiated.");

            Debug.Log("[Pool Test] Stopwatch: " + sw.ElapsedMilliseconds);
        }

        yield return new WaitForSeconds(2f);

        {
            Debug.Log("[Pool Test] Destroying...");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int index = 0; index < m_PoolObjects.Count; ++index)
            {
                GameObject go = m_PoolObjects[index];
                go.Recycle();
            }

            m_PoolObjects.Clear();

            Debug.Log("[Pool Test] Objects destroyed.");

            Debug.Log("[Pool Test] Stopwatch: " + sw.ElapsedMilliseconds);
        }

        Debug.Log("[Pool Test] End.");
    }
}
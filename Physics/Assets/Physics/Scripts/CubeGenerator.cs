using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CubeGenerator : MonoBehaviour
{
    // Serializable fields

    [SerializeField]
    private GameObject m_Prefab = null;
    [SerializeField]
    private int m_Instances = 10;
    [SerializeField]
    private Vector3 m_DistanceBetweenInstances = Vector3.one;

    // MonoBehaviour's interface

    private void Start()
    {
        CreateInstances();
    }

    // LOGIC

    public void CreateInstances()
    {
        if (m_Prefab == null)
            return;

        int instances = Mathf.Max(1, m_Instances);
        float exp = (1f / 3f);
        float floatSide = Mathf.Pow(instances, exp);
        int side = Mathf.RoundToInt(floatSide);

        Debug.Log("Side: " + side + " (" + (side * side * side) + " instances).");

        for (int xIndex = 0; xIndex < side; ++xIndex)
        {
            for (int yIndex = 0; yIndex < side; ++yIndex)
            {
                for (int zIndex = 0; zIndex < side; ++zIndex)
                {
                    CreateInstance(side, xIndex, yIndex, zIndex);
                }
            }
        }
    }

    // INTERNALS

    private void CreateInstance(int i_Side, int i_XIndex, int i_YIndex, int i_ZIndex)
    {
        if (m_Prefab == null)
            return;

        GameObject instance = Instantiate<GameObject>(m_Prefab);

        Transform instanceTransform = instance.transform;
        instanceTransform.parent = transform;

        Vector3 instancePosition = EvaluateInstancePosition(i_Side, i_XIndex, i_YIndex, i_ZIndex);

        instanceTransform.position = instancePosition;
    }

    private Vector3 EvaluateInstancePosition(int i_Side, int i_XIndex, int i_YIndex, int i_ZIndex)
    {
        float x = EvaluateInstanceComponentPosition(i_Side, i_XIndex, m_DistanceBetweenInstances.x);
        float y = EvaluateInstanceComponentPosition(i_Side, i_YIndex, m_DistanceBetweenInstances.y);
        float z = EvaluateInstanceComponentPosition(i_Side, i_ZIndex, m_DistanceBetweenInstances.z);

        return new Vector3(x, y, z);
    }

    private float EvaluateInstanceComponentPosition(int i_Side, int i_Index, float m_Distance)
    {
        float retValue = 0f;

        int halfSide = i_Side / 2;

        int delta = i_Index - halfSide;
        retValue = delta * m_Distance;

        return retValue;
    }
}
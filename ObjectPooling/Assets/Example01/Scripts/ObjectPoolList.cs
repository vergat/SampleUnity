using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class PoolDescriptor
{
    public GameObject prefab = null;
    public int size = 0;

    public bool allowRecycle = true;
}

public class ObjectPoolList : ScriptableObject
{
    [SerializeField]
    private List<PoolDescriptor> m_List = new List<PoolDescriptor>();

    public PoolDescriptor this[int i_Index]
    {
        get
        {
            return m_List[i_Index];
        }
        set
        {
            m_List[i_Index] = value;
        }
    }

    public int Count
    {
        get
        {
            return m_List.Count;
        }
    }
}

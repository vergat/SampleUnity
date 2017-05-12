using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPoolController : MonoBehaviour
{
    public List<ObjectPoolList> catalog = new List<ObjectPoolList>();

    private bool m_Loaded = false;

    // BUSINESS LOGIC

    public void LoadAll()
    {
        if (m_Loaded)
            return;

        for (int listIndex = 0; listIndex < catalog.Count; ++listIndex)
        {
            ObjectPoolList list = catalog[listIndex];
            LoadList(list);
        }

        m_Loaded = true;
    }

    public void UnloadAll()
    {
        if (!m_Loaded)
            return;

        m_Loaded = false;

        for (int listIndex = 0; listIndex < catalog.Count; ++listIndex)
        {
            ObjectPoolList list = catalog[listIndex];
            UnloadList(list);
        }
    }

    // INTERNALS

    private void LoadList(ObjectPoolList i_List)
    {
        if (i_List == null)
            return;

        for (int descriptorIndex = 0; descriptorIndex < i_List.Count; ++descriptorIndex)
        {
            PoolDescriptor descriptor = i_List[descriptorIndex];
            LoadPool(descriptor);
        }
    }

    private void UnloadList(ObjectPoolList i_List)
    {
        if (i_List == null)
            return;

        for (int descriptorIndex = 0; descriptorIndex < i_List.Count; ++descriptorIndex)
        {
            PoolDescriptor descriptor = i_List[descriptorIndex];
            UnloadPool(descriptor);
        }
    }

    private void LoadPool(PoolDescriptor i_Descriptor)
    {
        if (IsValid(i_Descriptor))
        {
            ObjectPool.CreatePoolMain(i_Descriptor.prefab, i_Descriptor.size, i_Descriptor.allowRecycle);
        }
    }

    private void UnloadPool(PoolDescriptor i_Descriptor)
    {
        if (IsValid(i_Descriptor))
        {
            ObjectPool.DestroyPooledMain(i_Descriptor.prefab);
        }
    }

    private bool IsValid(PoolDescriptor i_Descriptor)
    {
        return (i_Descriptor.prefab != null && i_Descriptor.size > 0);
    }
}

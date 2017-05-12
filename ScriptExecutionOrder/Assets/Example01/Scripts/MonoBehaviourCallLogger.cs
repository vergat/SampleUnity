using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MonoBehaviourCallLogger : MonoBehaviour
{
    // Serializable fields

    [Header("Calls")]

    [SerializeField]
    private bool m_Awake = true;

    [SerializeField]
    private bool m_OnEnable = true;
    [SerializeField]
    private bool m_OnDisable = true;

    [SerializeField]
    private bool m_Start = true;

    [SerializeField]
    private bool m_Update = true;
    [SerializeField]
    private bool m_FixedUpdate = true;
    [SerializeField]
    private bool m_LateUpdate = true;

    [SerializeField]
    private bool m_OnDestroy = true;

    // MonoBehaviour's interface

    private void Awake()
    {
        if (m_Awake)
        {
            Debug.Log("[" + gameObject.name + "] Awake");
        }
    }

    private void OnEnable()
    {
        if (m_OnEnable)
        {
            Debug.Log("[" + gameObject.name + "] OnEnable");
        }
    }

    private void OnDisable()
    {
        if (m_OnDisable)
        {
            Debug.Log("[" + gameObject.name + "] OnDisable");
        }
    }

    private void Start()
    {
        if (m_Start)
        {
            Debug.Log("[" + gameObject.name + "] Start");
        }
    }

    private void Update()
    {
        if (m_Update)
        {
            Debug.Log("[" + gameObject.name + "] Update");
        }
    }

    private void FixedUpdate()
    {
        if (m_FixedUpdate)
        {
            Debug.Log("[" + gameObject.name + "] FixedUpdate");
        }
    }

    private void LateUpdate()
    {
        if (m_LateUpdate)
        {
            Debug.Log("[" + gameObject.name + "] LateUpdate");
        }
    }

    private void OnDestroy()
    {
        if (m_OnDestroy)
        {
            Debug.Log("[" + gameObject.name + "] OnDestroy");
        }
    }
}
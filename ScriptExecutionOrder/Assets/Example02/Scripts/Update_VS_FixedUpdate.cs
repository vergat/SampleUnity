using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Update_VS_FixedUpdate : MonoBehaviour
{
    // Serializable fields

    [SerializeField]
    private KeyCode m_Key = KeyCode.Space;

    // Fields

    private bool m_ButtonDownRequest = false;

    private int m_CounterA = 0;
    private int m_CounterB = 0;

    private event Action<int> m_OnCounterAChanged = null;
    private event Action<int> m_OnCounterBChanged = null;

    // ACCESSORS

    public int counterA
    {
        get { return m_CounterA; }
    }

    public int counterB
    {
        get { return m_CounterB; }
    }

    public event Action<int> onCounterAChanged
    {
        add { m_OnCounterAChanged += value; }
        remove { m_OnCounterAChanged -= value; }
    }

    public event Action<int> onCounterBChanged
    {
        add { m_OnCounterBChanged += value; }
        remove { m_OnCounterBChanged -= value; }
    }

    // MonoBehaviour's interface

    private void Update()
    {
        bool keyDown = Input.GetKeyDown(m_Key);
        if (keyDown)
        {
            m_ButtonDownRequest = true;
        }
    }

    private void FixedUpdate()
    {
        bool keyDown = Input.GetKeyDown(m_Key);
        if (keyDown)
        {
            ++m_CounterA;
            if (m_OnCounterAChanged != null)
            {
                m_OnCounterAChanged(m_CounterA);
            }
        }

        if (m_ButtonDownRequest)
        {
            ++m_CounterB;
            if (m_OnCounterBChanged != null)
            {
                m_OnCounterBChanged(m_CounterB);
            }

            m_ButtonDownRequest = false;
        }
    }
}
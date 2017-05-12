using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Update_VS_FixedUpdate_UI : MonoBehaviour
{
    // Serializable fields

    [Header("Widgets")]

    [SerializeField]
    private Text m_CounterA = null;
    [SerializeField]
    private Text m_CounterB = null;

    [Header("Component")]

    [SerializeField]
    private Update_VS_FixedUpdate m_Target = null;

    // MonoBehaviour's interface

    private void OnEnable()
    {
        RegisterEvents();

        UpdateView();
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    // LOGIC

    public void SetTarget(Update_VS_FixedUpdate i_Target)
    {
        UnregisterEvents();

        m_Target = i_Target;

        RegisterEvents();

        UpdateView();
    }

    // INTERNALS

    private void RegisterEvents()
    {
        if (m_Target != null)
        {
            m_Target.onCounterAChanged += OnCounterAChangedEvent;
            m_Target.onCounterBChanged += OnCounterBChangedEvent;
        }
    }

    private void UnregisterEvents()
    {
        if (m_Target != null)
        {
            m_Target.onCounterAChanged -= OnCounterAChangedEvent;
            m_Target.onCounterBChanged -= OnCounterBChangedEvent;
        }
    }

    private void UpdateView()
    {
        int counterA = (m_Target != null) ? m_Target.counterA : 0;
        int counterB = (m_Target != null) ? m_Target.counterB : 0;

        if (m_CounterA != null)
        {
            m_CounterA.text = counterA.ToString();
        }

        if (m_CounterB != null)
        {
            m_CounterB.text = counterB.ToString();
        }
    }

    // EVENTS

    private void OnCounterAChangedEvent(int i_NewValue)
    {
        UpdateView();
    }

    private void OnCounterBChangedEvent(int i_NewValue)
    {
        UpdateView();
    }
}
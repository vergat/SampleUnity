using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    // Serializable fields

    [Header("Input")]

    [SerializeField]
    private string m_HorizontalAxisName = "Horizontal";
    [SerializeField]
    private string m_VerticalAxisName = "Vertical";

    [Header("Movement logic")]

    [SerializeField]
    private float m_Force = 100;
    [SerializeField]
    private float m_MaxSpeed = 10f;

    // Fields

    private float m_HorizontalAxis = 0f;
    private float m_VerticalAxis = 0f;

    // Components

    private Rigidbody m_Rigidbody = null;

    // MonoBehaviour's interface

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxis(m_HorizontalAxisName);
        float vertical = Input.GetAxis(m_VerticalAxisName);

        m_HorizontalAxis = horizontal;
        m_VerticalAxis = vertical;
    }

    private void FixedUpdate()
    {
        Vector3 forward = transform.forward;
        Vector3 forwardForce = forward * m_VerticalAxis * m_Force;

        Vector3 right = transform.right;
        Vector3 rightForce = right * m_HorizontalAxis * m_Force;

        Vector3 appliedForce = forwardForce + rightForce;

        if (m_MaxSpeed > 0f)
        {
            Vector3 currentVelocity = m_Rigidbody.velocity;

            Vector3 acceleration = appliedForce / m_Rigidbody.mass;         // F = m * a    ==> a = F / m
            Vector3 deltaVelocity = acceleration * Time.fixedDeltaTime;     // a = dv / dt  ==> dv = a * dt

            Vector3 newVelocity = currentVelocity + deltaVelocity;

            if (newVelocity.sqrMagnitude > m_MaxSpeed * m_MaxSpeed)
            {
                // Modulate force.

                Vector3 maxVelocity = newVelocity.normalized * m_MaxSpeed;
                Vector3 maxAcceleration = (maxVelocity - currentVelocity) / Time.fixedDeltaTime;

                appliedForce = maxAcceleration * m_Rigidbody.mass;
            }
        }

        m_Rigidbody.AddForce(appliedForce);
    }
}
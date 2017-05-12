using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerJumpBugged : MonoBehaviour
{
    private static string s_GroundTag = "Ground";

    // Serializable fields

    [Header("Input")]

    [SerializeField]
    private string m_JumpAction = "Jump";

    [Header("Jump logic")]

    [SerializeField]
    private float m_Force = 450f;
    [SerializeField]
    private Transform m_GroundCheckPivot = null;
    [SerializeField]
    private float m_GroundCheckRadius = 0.1f;

    // Components

    private Rigidbody m_Rigidbody = null;

    // ACCESSORS

    public Vector3 groundCheckPivotPosition
    {
        get
        {
            if (m_GroundCheckPivot == null)
            {
                return transform.position;
            }

            return m_GroundCheckPivot.position;
        }
    }

    public float groundCheckRadius
    {
        get
        {
            float v = Mathf.Max(0f, m_GroundCheckRadius);
            return v;
        }
    }

    // MonoBehaviour's interface

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        bool buttonDown = Input.GetButtonDown(m_JumpAction);
        if (buttonDown)
        {
            Vector3 up = transform.up;
            Vector3 jumpForce = up * m_Force;

            bool isGrounded = IsGrounded();

            if (isGrounded)
            {
                m_Rigidbody.AddForce(jumpForce);
            }
        }
    }

    // INTERNALS

    private bool IsGrounded()
    {
        Collider[] colliders = Physics.OverlapSphere(groundCheckPivotPosition, groundCheckRadius);

        for (int colliderIndex = 0; colliderIndex < colliders.Length; ++colliderIndex)
        {
            Collider currentCollider = colliders[colliderIndex];
            GameObject go = currentCollider.gameObject;
            string tag = go.tag;
            if (tag == s_GroundTag)
            {
                return true;
            }
        }

        return false;
    }
}
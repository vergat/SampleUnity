using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Logic")]

    [SerializeField]
    private Transform m_Target = null;
    [SerializeField]
    private bool m_FollowY = false;

    [SerializeField]
    [Range(0f, 1f)]
    private float m_PositionLerpFactor = 0.9f;

    // ACCESSORS

    public bool followY
    {
        get { return m_FollowY; }
        set { m_FollowY = value; }
    }

    // MonoBehaviour's interface

    private void LateUpdate()
    {
        UpdatePosition();
    }

    // INTERNALS

    private void UpdatePosition()
    {
        if (m_Target == null)
            return;

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = m_Target.position;

        if (!m_FollowY)
        {
            targetPosition.y = currentPosition.y;
        }

        Vector3 nextPosition = Vector3.Lerp(currentPosition, targetPosition, m_PositionLerpFactor);
        transform.position = nextPosition;
    }

    // LOGIC

    public void SetTarget(GameObject i_GameObject)
    {
        Transform target = null;

        if (i_GameObject != null)
        {
            target = i_GameObject.transform;
        }

        SetTarget(target);
    }

    public void SetTarget(Transform i_Target)
    {
        m_Target = i_Target;
    }
}
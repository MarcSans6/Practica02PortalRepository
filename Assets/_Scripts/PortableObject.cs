using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PortableObject : MonoBehaviour
{
    public bool m_LockPosition = false;
    public bool m_LockRotation = false;
    public bool m_LockScale = false;
    public bool m_LockVelocity = false;

    public Vector3 CenterPos => m_CenterPosition.position;
    [SerializeField] protected Transform m_CenterPosition;
    [Range(0.0f,1.0f)]
    //private int m_PortalCount = 0;

    protected Portal m_InPortal;
    protected Portal m_OutPortal;

    protected Rigidbody m_Rigidbody;
    private Collider m_Collider;
    private bool m_CanWarp = true;

    protected static readonly Quaternion m_HalfTurn = Quaternion.Euler(.0f, 180.0f, .0f);
    protected virtual void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<Collider>();
        if (m_CenterPosition == null)
        {
            m_CenterPosition = transform;
        }

    }
    public void SetIsInPortal(Portal _InPortal, Portal _OutPortal, Collider _WallCollider)
    {
        m_InPortal = _InPortal;
        m_OutPortal = _OutPortal;
        Physics.IgnoreCollision(m_Collider, _WallCollider, true);
    }

    public void  ExitPortal(Collider _WallCollider)
    {
        Physics.IgnoreCollision(m_Collider, _WallCollider, false);
    }

    public bool CanWarp()
    {
        return m_CanWarp;
    }

    public virtual void Warp()
    {
        var l_InTransform = m_InPortal.transform;
        var l_OutTransform = m_OutPortal.transform;

        //Update position
        if (!m_LockPosition)
        {
            Vector3 l_RelativePos = l_InTransform.InverseTransformPoint(transform.position);
            l_RelativePos = m_HalfTurn * l_RelativePos;
            transform.position = l_OutTransform.TransformPoint(l_RelativePos);
        }

        //Update rotation
        if (!m_LockRotation)
        {
            Quaternion l_RelativeRot = Quaternion.Inverse(l_InTransform.rotation) * transform.rotation;
            l_RelativeRot = m_HalfTurn * l_RelativeRot;
            transform.rotation = l_OutTransform.rotation * l_RelativeRot;
        }

        //Update scale
        if (!m_LockScale)
        {
            Vector3 l_Scale = transform.localScale;
            float l_ScaleFraction = l_OutTransform.localScale.x / l_InTransform.localScale.x;
            l_Scale *= l_ScaleFraction;
            transform.localScale = l_Scale;
            m_Rigidbody.velocity = (m_Rigidbody.velocity.normalized) * m_Rigidbody.velocity.magnitude * transform.localScale.x;
        }

        //Update velocity of rigidbody

        if (!m_LockVelocity)
        {
            Vector3 l_RelativeVel = l_InTransform.InverseTransformDirection(m_Rigidbody.velocity);
            l_RelativeVel = m_HalfTurn * l_RelativeVel;
            m_Rigidbody.velocity = l_OutTransform.TransformDirection(l_RelativeVel);
        }

        //Calls the AfterWarp, in case any subclass wants to use it.
        AfterWarp(l_InTransform, l_OutTransform);

        //Swap portal references in case object doesn't exit the collider
        var l_Tmp = m_InPortal;
        m_InPortal = m_OutPortal;
        m_OutPortal = l_Tmp;
    }

    public void SetCanWarp(bool v)
    {
        m_CanWarp = v;
    }

    protected virtual void AfterWarp(Transform _InTransform, Transform _OutTransform) { }
}

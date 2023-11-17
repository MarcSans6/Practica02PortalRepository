using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PortableObject : MonoBehaviour, IRestartLevelElement
{
    public bool m_LockPosition = false;
    public bool m_LockRotation = false;
    public bool m_LockScale = false;
    public bool m_LockVelocity = false;
    public GameObject m_ClonePrefab;
    private GameObject m_Clone;

    bool m_StartLockPos;
    bool m_StartLockRot;
    bool m_StartLockScale;
    bool m_StartLockVel;

    public Vector3 CenterPos => m_CenterPosition.position;
    [SerializeField] protected Transform m_CenterPosition;
    [Range(0.0f,1.0f)]
    //private int m_PortalCount = 0;

    protected Portal m_InPortal;
    protected Portal m_OutPortal;

    protected Rigidbody m_Rigidbody;
    private Collider m_Collider;
    private bool m_CanWarp = true;
    bool m_StartCanWarp;

    protected static readonly Quaternion m_HalfTurn = Quaternion.Euler(.0f, 180.0f, .0f);


    protected virtual void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<Collider>();
        if (m_CenterPosition == null)
        {
            m_CenterPosition = transform;
        }

        m_StartLockPos = m_LockPosition;
        m_StartLockRot = m_LockRotation;
        m_StartLockScale = m_LockScale;
        m_StartLockVel = m_LockVelocity;
        m_StartCanWarp = m_CanWarp;

        //Create clone
        if (m_ClonePrefab != null)
            m_Clone = GameObject.Instantiate(m_ClonePrefab, new Vector3(-1000.0f, 1000.0f, -1000.0f), transform.rotation);
        

    }
    private void Start()
    {
        GameController.GetGameController().AddRestartLevelElement(this);
    }

    private void LateUpdate()
    {
        if (m_InPortal == null || m_OutPortal == null)
        {
            return;
        }

        if (m_Clone.activeSelf && m_InPortal.IsPlaced && m_OutPortal.IsPlaced)
        {
            var l_InTransform = m_InPortal.transform;
            var l_OutTransform= m_OutPortal.transform;

            //Update position clone
            if (true)
            {
                Vector3 l_RelativePos = l_InTransform.InverseTransformPoint(transform.position);
                l_RelativePos = m_HalfTurn * l_RelativePos;
                m_Clone.transform.position = l_OutTransform.TransformPoint(l_RelativePos);
            }

            if (true)
            {
                Quaternion l_RelativeRot = Quaternion.Inverse(l_InTransform.rotation) * transform.rotation;
                l_RelativeRot = m_HalfTurn * l_RelativeRot;
                m_Clone.transform.rotation = l_OutTransform.rotation * l_RelativeRot;
            }
        }
        else
        {
            m_Clone.transform.position = new Vector3(-1000.0f, 1000.0f, -1000.0f);
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

    public void RestartElement()
    {
        m_LockPosition = m_StartLockPos;
        m_LockRotation = m_StartLockRot;
        m_LockScale = m_StartLockScale;
        m_LockVelocity = m_StartLockVel;
        m_CanWarp = m_StartCanWarp;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PortableObject : MonoBehaviour
{
    public Vector3 CenterPos => m_CenterPosition.position;
    [SerializeField] protected Transform m_CenterPosition;
    [Range(0.0f,1.0f)]
    [SerializeField] protected float m_MinDotToWarp = 40.0f;
    //private int m_PortalCount = 0;

    private Portal m_InPortal;
    private Portal m_OutPortal;

    private new Rigidbody m_Rigidbody;
    protected new Collider m_Collider;
    private bool m_CanWarp = true;

    private static readonly Quaternion m_HalfTurn = Quaternion.Euler(.0f, 180.0f, .0f);
    protected virtual void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<Collider>();

        //Portal.OnPlaced += OnPortalPlaced;
    }

    //public void OnPortalPlaced(Portal _Portal)
    //{
    //    if (m_InPortal!= null || m_OutPortal != null)
    //    {
    //        if (_Portal == m_InPortal || _Portal == m_OutPortal)
    //        {
    //            m_CanWarp = false;
    //            ExitPortal(_Portal.WallCollider);
    //        }
    //    }
    //}

    private void LateUpdate()
    {
        //if (m_InPortal == null || m_OutPortal == null)
        //{
        //    return;
        //}

        //UPDATE CLONE OBJECT, JA VEUREM....


    }

    public void SetIsInPortal(Portal _InPortal, Portal _OutPortal, Collider _WallCollider)
    {
        m_InPortal = _InPortal;
        m_OutPortal = _OutPortal;
        Physics.IgnoreCollision(m_Collider, _WallCollider, true);
        Debug.Log("Ignored Collisions with " + _WallCollider.name);
        //++m_PortalCount;
    }

    public void  ExitPortal(Collider _WallCollider)
    {
        Physics.IgnoreCollision(m_Collider, _WallCollider, false);
        //--m_PortalCount;

        //if (m_PortalCount == 0)
        //{
        //    //Deactivate clone object;
        //}
    }

    public bool CanWarp(Portal _Portal)
    {
        return m_CanWarp;

        Vector3 l_PortalEnteringDirection = _Portal.m_OtherPortal.transform.forward;
        Vector3 l_Forward = m_Rigidbody.velocity.normalized;
        if (_Portal.IsInHorizontalRotation())
        {
            return Mathf.Sign(l_PortalEnteringDirection.y) == Mathf.Sign(l_Forward.y);
        }
        else
        {
            Debug.Log("FuckedSituation");
            l_PortalEnteringDirection.y = 0;
            l_Forward.y = 0;
            return Vector3.Dot(l_Forward, l_PortalEnteringDirection) >= m_MinDotToWarp;
        }

        
    }

    public void Warp()
    {
        var l_InTransform = m_InPortal.transform;
        var l_OutTransform = m_OutPortal.transform;

        //Update position
        Vector3 l_RelativePos = l_InTransform.InverseTransformPoint(transform.position);
        l_RelativePos = m_HalfTurn * l_RelativePos;
        transform.position = l_OutTransform.TransformPoint(l_RelativePos);

        //Update rotation
        Quaternion l_RelativeRot = Quaternion.Inverse(l_InTransform.rotation) * transform.rotation;
        l_RelativeRot = m_HalfTurn * l_RelativeRot;
        transform.rotation = l_OutTransform.rotation * l_RelativeRot;

        //Update velocity of rigidbody

        Vector3 l_RelativeVel = l_InTransform.InverseTransformDirection(m_Rigidbody.velocity);
        l_RelativeVel = m_HalfTurn * l_RelativeVel;
        m_Rigidbody.velocity = l_OutTransform.TransformDirection(l_RelativeVel);

        //Swap portal references
        var l_Tmp = m_InPortal;
        m_InPortal = m_OutPortal;
        m_OutPortal = l_Tmp;
        //Debug.Break();
    }

    public void SetCanWarp(bool v)
    {
        m_CanWarp = v;
    }
}

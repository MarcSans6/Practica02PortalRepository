using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Rigidbody))]
public class AttachableObject : MonoBehaviour
{
    [Header("Attach")]
    public float m_AttachedDrag = 10;
    private float m_DeatachedDrag;
    public float m_MinDistanceToTarget = 0.1f;
    private float m_AttachForce;
    Rigidbody m_Rigidbody;
    Transform m_TargetTransform;
    PortableObject m_PortableObject;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_PortableObject = GetComponent<PortableObject>();
    }

    void Update()
    {
        if (m_TargetTransform != null)
        {
            MoveAttachedObject();
        }
    }

    private void MoveAttachedObject()
    {
        float l_DistanceToTarget = Vector3.Distance(transform.position, m_TargetTransform.position);
        if (l_DistanceToTarget > m_MinDistanceToTarget)
        {
            Vector3 l_MoveDirection = (m_TargetTransform.position - transform.position).normalized;
            m_Rigidbody.AddForce(l_MoveDirection * m_AttachForce);
        }
    }

    public void Attach(Transform _Target, float _AttachForce)
    {
        m_TargetTransform = _Target;
        m_AttachForce = _AttachForce;
        m_Rigidbody.useGravity = false;
        m_DeatachedDrag = m_Rigidbody.drag;
        m_Rigidbody.drag = m_AttachedDrag;
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        transform.SetParent(_Target);

        //IGNORE COLLISIONS WITH PLAYER

        if (m_PortableObject!= null)
        {
            m_PortableObject.SetCanWarp(false);
            //MAKE LOCK POS, LOCK ROT AND LOCK, ETC,ETC
        }
    }

    public void Deattach(float _Force, Vector3 _Dir)
    {
        m_TargetTransform = null;
        m_Rigidbody.useGravity = true;
        m_Rigidbody.drag = m_DeatachedDrag;
        m_Rigidbody.constraints = RigidbodyConstraints.None;
        transform.SetParent(null);
        if (m_PortableObject != null)
        {
            m_PortableObject.SetCanWarp(false);
        }

        m_Rigidbody.velocity = _Force * _Dir.normalized;
    }
}

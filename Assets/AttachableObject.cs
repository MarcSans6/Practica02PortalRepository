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
    public float m_MinDistanceToTarget = 0.5f;
    private float m_AttachForce;
    public bool m_AttachLookingForward = false;
    public float m_RotationSpeed;

    Rigidbody m_Rigidbody;
    Transform m_TargetTransform;
    PortableObject m_PortableObject;
    Collider m_Collider;
    Collider m_GunUserCollider;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_PortableObject = GetComponent<PortableObject>();
        m_Collider = GetComponent<Collider>();
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

        if (m_AttachLookingForward)
        {
            Quaternion l_TargetRotation = m_TargetTransform.rotation;
            Quaternion l_Rotation = transform.rotation;

            l_Rotation = Quaternion.RotateTowards(l_Rotation, l_TargetRotation, m_RotationSpeed * Time.fixedDeltaTime);

            transform.rotation = l_Rotation;
        }
    }

    public void Attach(Transform _Target, float _AttachForce, Collider _GunUserCollider)
    {
        m_TargetTransform = _Target;
        m_AttachForce = _AttachForce;
        m_Rigidbody.useGravity = false;
        m_DeatachedDrag = m_Rigidbody.drag;
        m_Rigidbody.drag = m_AttachedDrag;
        
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        
        transform.SetParent(_Target);

        m_GunUserCollider = _GunUserCollider;
        Physics.IgnoreCollision(m_GunUserCollider, m_Collider, true);

        //IGNORE COLLISIONS WITH PLAYER

        if (m_PortableObject!= null)
        {
            m_PortableObject.m_LockPosition = true;
            m_PortableObject.m_LockRotation= true;
            m_PortableObject.m_LockVelocity= true;
        }
    }

    public void Deattach(float _Force, Vector3 _Dir)
    {
        m_TargetTransform = null;
        m_Rigidbody.useGravity = true;
        m_Rigidbody.drag = m_DeatachedDrag;
        m_Rigidbody.constraints = RigidbodyConstraints.None;
        transform.SetParent(null);
        Physics.IgnoreCollision(m_GunUserCollider, m_Collider, false);

        if (m_PortableObject != null)
        {
            m_PortableObject.m_LockPosition = false;
            m_PortableObject.m_LockRotation = false;
            m_PortableObject.m_LockVelocity = false;
        }

        m_Rigidbody.velocity = _Force * _Dir.normalized;
    }

    internal bool IsAttached()
    {
        return m_TargetTransform != null;
    }
}

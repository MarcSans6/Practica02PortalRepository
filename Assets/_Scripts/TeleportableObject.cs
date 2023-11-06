using UnityEngine;
using System;

public class TeleportableObject : Teleportable
{
    protected Rigidbody m_RigidBody;

    protected void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
    }
    public override bool CanTeleport(Portal _Portal)
    {
        m_Forward = transform.forward;
        return base.CanTeleport(_Portal);
    }

    public override void Teleport(Portal _Portal)
    {
        m_Forward = m_RigidBody.velocity.normalized;
        Vector3 l_LocalVelocity = _Portal.m_OtherPortal.InverseTransformDirection(m_RigidBody.velocity);
        Vector3 l_WorldVelocity = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalVelocity);
        Vector3 l_ForwardVelocity = l_WorldVelocity;
        l_ForwardVelocity.Normalize();
        base.Teleport(_Portal);
        m_RigidBody.velocity = l_WorldVelocity;

    }
}


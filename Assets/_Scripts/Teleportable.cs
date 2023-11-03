using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public abstract class Teleportable: MonoBehaviour
{
    [Header("Parameters")]
    [Range(0.0f, 1.0f)]
    public float m_DotTraversePortal = 0.42f;
    public float m_PortalOffset = 0.8f;
    protected Vector3 m_Forward;

    public virtual bool CanTeleport(Portal _Portal)
    {
        float l_DotAngle = Vector3.Dot(_Portal.m_OtherPortal.forward, m_Forward);
        return l_DotAngle > m_DotTraversePortal;
    }

    public virtual void Teleport(Portal _Portal)
    {
        Vector3 l_LocalPosition = _Portal.m_OtherPortal.InverseTransformPoint(transform.position);
        Vector3 l_WorldPosition = _Portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition);
        Vector3 l_LocalMovement = _Portal.m_OtherPortal.InverseTransformDirection(m_Forward);
        Vector3 l_WorldMovement = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalMovement);
        transform.position = l_WorldPosition + l_WorldMovement * m_PortalOffset;
    }
}


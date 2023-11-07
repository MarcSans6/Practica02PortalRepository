using System.Security.Cryptography;
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

    protected Vector3 m_RotatedForward;

    public virtual bool CanTeleport(Portal _Portal)
    {
        Debug.Log("CanTleporti=");
        m_RotatedForward = _Portal.m_OtherPortal.rotation * transform.forward;
        float l_DotAngle = Vector3.Dot(_Portal.m_OtherPortal.forward, m_RotatedForward);
        return l_DotAngle > m_DotTraversePortal && _Portal.m_MirrorPortal.isActiveAndEnabled;
        return false;
    }

    public virtual void Teleport(Portal _Portal)
    {
        Vector3 l_LocalPosition = _Portal.m_OtherPortal.InverseTransformPoint(transform.position);
        Vector3 l_WorldPosition = _Portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition);
        Vector3 l_LocalMovement = _Portal.m_OtherPortal.InverseTransformDirection(m_Forward);
        Vector3 l_WorldMovement = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalMovement);
        transform.position = l_WorldPosition + l_WorldMovement * m_PortalOffset;
    }

    private void Update()
    {
        Debug.DrawLine(transform.position, m_Forward * 3);
        Gizmos.color = Color.blue;
        Debug.DrawLine(transform.position, m_RotatedForward * 3);
    }

    private void OnDrawGizmos()
    {
        Vector3 startPos1 = transform.position + Vector3.up * 1;
        Vector3 startPos2 = transform.position + Vector3.up * 2;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(startPos1, startPos1 + m_Forward * 3);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(startPos2, startPos2 + m_RotatedForward * 3);
    }
}


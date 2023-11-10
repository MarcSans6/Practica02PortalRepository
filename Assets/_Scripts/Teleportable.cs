using System.Security.Cryptography;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(Rigidbody))]
public class Teleportable: MonoBehaviour
{
    [Header("Parameters")]
    [Range(0.0f, 1.0f)]
    public float m_DotTraversePortal = 0.42f;
    public float m_PortalOffset = 0.8f;
    protected Vector3 m_Forward;
    protected Rigidbody m_Rigidbody;

    protected virtual void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public virtual bool CanTeleport(Portal _Portal)
    {
        if (_Portal.IsHorizontal())
        {
            return _Portal.m_MirrorPortal.isActiveAndEnabled;
        }
        Vector3 l_RotatedForward = new(m_Forward.x, -Mathf.Tan(_Portal.m_OtherPortal.rotation.eulerAngles.y), m_Forward.z);
        Debug.Log("Can teleport");
        float l_DotAngle = Vector3.Dot(_Portal.m_OtherPortal.forward, l_RotatedForward);
        return l_DotAngle > m_DotTraversePortal && _Portal.m_MirrorPortal.isActiveAndEnabled;
    }

    public virtual void Teleport(Portal _Portal)
    {
        Vector3 l_LocalPosition = _Portal.m_OtherPortal.InverseTransformPoint(transform.position);
        Vector3 l_WorldPosition = _Portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition);
        Vector3 l_LocalDir = _Portal.m_OtherPortal.InverseTransformDirection(m_Forward);
        Vector3 l_WorldDir = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalDir);
        Vector3 l_Position = l_WorldPosition + l_WorldDir * m_PortalOffset;
        m_Rigidbody.MovePosition(l_Position);
        //Debug.Break();
        Vector3 l_LocalVelocity = _Portal.m_OtherPortal.InverseTransformDirection(m_Rigidbody.velocity);
        Vector3 l_WorldVelocity = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalVelocity);
        m_Rigidbody.velocity = l_WorldVelocity;
        //Debug.Break();
    }
}


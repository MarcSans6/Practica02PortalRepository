using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPreview : MonoBehaviour
{
    [Header("References")]
    public List<Transform> m_ValidPoints;
    public Portal m_Portal;
    [Space]
    public float m_MinDistanceToMirrorPortal;
    public float m_MinDistanceToValidPoints;
    public float m_MaxDistanceToValidPoints;
    public float m_ValidPointsOffset;
    public float m_MinValidDotAngle;

    public Collider WallCollider => m_WallCollider;
    Collider m_WallCollider;

    public bool IsValid => m_IsValid;
    private bool m_IsValid;

    private void Awake()
    {
        SetShow(false);
    }

    public bool IsValidPosition(Vector3 ShootPosition, Vector3 Position, Vector3 Normal, LayerMask _LayerMask)
    {
        SetTransform(Position, Normal, ShootPosition);
        m_IsValid = true;

        for (int i = 0; i < m_ValidPoints.Count; i++)
        {
            Vector3 l_Direction = m_ValidPoints[i].position - ShootPosition;
            float l_Distance = l_Direction.magnitude;
            l_Direction.Normalize();
            Ray l_Ray = new Ray(ShootPosition, l_Direction);
            RaycastHit l_RayCastHit;
            //We check if the mirror portal is close to this valid point, just if both portals are facing the same direction
            if (m_Portal.m_MirrorPortal.isActiveAndEnabled && m_Portal.m_MirrorPortal.transform.forward == transform.forward)
            {
                float l_DistanceToMirrorPortal = Vector3.Distance(m_ValidPoints[i].position, m_Portal.m_MirrorPortal.transform.position);
                if (l_DistanceToMirrorPortal < m_MinDistanceToMirrorPortal)
                {
                    m_IsValid = false;
                }
            }
            if (Physics.Raycast(l_Ray, out l_RayCastHit, l_Distance + m_ValidPointsOffset, _LayerMask.value))
            {

                if (l_RayCastHit.collider.tag == "Drawable")
                {
                    float l_DistanceToHit = Vector3.Distance(m_ValidPoints[i].position, l_RayCastHit.point);
                    if (l_DistanceToHit >= m_MinDistanceToValidPoints && l_DistanceToHit <= m_MaxDistanceToValidPoints)
                    {
                        float l_DotAngle = Vector3.Dot(Normal, l_RayCastHit.normal);
                        if (l_DotAngle < m_MinValidDotAngle)
                        {
                            m_IsValid = false;
                        }
                    }
                    else
                        m_IsValid = false;
                }
                else
                    m_IsValid = false;
            }
            else
                m_IsValid = false;
        }
        return m_IsValid;
    }

    internal void SetShow(bool _Show)
    {
        gameObject.SetActive(_Show);
    }

    public void SetWallCollider(Collider _WallCollider)
    {
        m_WallCollider = _WallCollider;
    }

    private void SetTransform(Vector3 _Position, Vector3 _Normal, Vector3 _ShootPosition)
    {
        transform.position = _Position;
        transform.rotation = Quaternion.LookRotation(_Normal);
        if (_Normal == Vector3.up || _Normal == Vector3.down)
        {
            Vector3 l_ShootDir = _Position - _ShootPosition;
            l_ShootDir.Normalize();
            Quaternion l_DirQuaternion = Quaternion.LookRotation(-l_ShootDir);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, l_DirQuaternion.eulerAngles.y);
        }
    }
}

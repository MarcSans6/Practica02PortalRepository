using System.Collections.Generic;
using UnityEngine;

public class Portal: MonoBehaviour
{
    public Camera m_Camera;
    PlayerController m_playerController;
    public Transform m_OtherPortal;
    public Portal m_MirrorPortal;
    public float m_OffsetNearPlane = 0.1f;

    public List<Transform> m_ValidPoints;
    public float m_MinDistanceToValidPoints;
    private float m_MaxDistanceToValidPoints;
    public float m_ValidPointsOffset = 0.1f;
    public float m_MinValidDotAngle = 0.95f;

    private void Start()
    {
        m_playerController = GameManager.GetGameManager().m_Player;
    }

    private void LateUpdate()
    {
        Vector3 l_localPosition = m_OtherPortal.InverseTransformPoint(m_playerController.m_Camera.transform.position);
        Vector3 l_WorldPosition = m_MirrorPortal.transform.TransformPoint(l_localPosition);
        m_MirrorPortal.m_Camera.transform.position = l_WorldPosition;
        Vector3 l_localDirections = m_OtherPortal.InverseTransformDirection(m_playerController.m_Camera.transform.forward);
        Vector3 l_WorldDirection = m_MirrorPortal.transform.TransformDirection(l_localDirections);
        m_MirrorPortal.m_Camera.transform.forward = l_WorldDirection;

        float l_Distance = Vector3.Distance(l_WorldPosition, m_MirrorPortal.transform.position) + m_OffsetNearPlane;
        m_MirrorPortal.m_Camera.nearClipPlane = l_Distance;

    }

    public bool IsValidPosition(Vector3 ShootPosition, Vector3 Position, Vector3 Normal,LayerMask _LayerMask )
    {
        transform.position = Position;
        transform.rotation=Quaternion.LookRotation(Normal);
        bool l_IsValid = true;
        for (int i = 0; i < m_ValidPoints.Count; i++)
        {
            Vector3 l_Direction = m_ValidPoints[i].position - ShootPosition;
            float l_Distance = l_Direction.magnitude;
            l_Direction.Normalize();
            Ray l_Ray = new Ray(ShootPosition, l_Direction);
            RaycastHit l_RayCastHit;
            if(Physics.Raycast(l_Ray, out l_RayCastHit, l_Distance + m_ValidPointsOffset, _LayerMask.value))
            {
                if (l_RayCastHit.collider.tag == "Drawable")
                {
                    float l_DistanceToHIt = Vector3.Distance(m_ValidPoints[i].position, l_RayCastHit.point);
                    if (l_DistanceToHIt >= m_MinDistanceToValidPoints && l_DistanceToHIt <= m_MaxDistanceToValidPoints)
                    {
                        float l_DotAngle = Vector3.Dot(Normal, l_RayCastHit.normal);
                        if (l_DotAngle < m_MinValidDotAngle)
                        {
                            l_IsValid = false;
                        }
                    }
                    else 
                        l_IsValid = false;
                }
                else 
                    l_IsValid = false;

            }
            else 
                l_IsValid = false;
        }
        return l_IsValid;
    }
}

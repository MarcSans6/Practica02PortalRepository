using System.Collections.Generic;
using UnityEngine;

public class Portal: MonoBehaviour
{
    public Camera m_Camera;
    FPSController m_FPSController;
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
        m_FPSController = GameController.GetGameController().m_Player.GetComponent<FPSController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Teleportable l_Teleportable = other.GetComponent<Teleportable>();
        if (l_Teleportable != null)
        {
            if (l_Teleportable.CanTeleport(this))
            {
                l_Teleportable.Teleport(this);
            }
        }
    }

    private void LateUpdate()
    {
        UpdateMirrorPortalCamera();
    }

    private void UpdateMirrorPortalCamera()
    {
        //PlayerCamera world position to local position from otherPortal;
        Vector3 l_localPosition = m_OtherPortal.InverseTransformPoint(m_FPSController.Camera.transform.position);
        //Applies l_localPosition to the MirrorPortal, then transforms that position into WorldSpace;
        Vector3 l_WorldPosition = m_MirrorPortal.transform.TransformPoint(l_localPosition);
        m_MirrorPortal.m_Camera.transform.position = l_WorldPosition;

        Vector3 l_localDirections = m_OtherPortal.InverseTransformDirection(m_FPSController.Camera.transform.forward);
        Vector3 l_WorldDirection = m_MirrorPortal.transform.TransformDirection(l_localDirections);
        m_MirrorPortal.m_Camera.transform.forward = l_WorldDirection;

        float l_Distance = Vector3.Distance(l_WorldPosition, m_MirrorPortal.transform.position) + m_OffsetNearPlane;
        m_MirrorPortal.m_Camera.nearClipPlane = l_Distance;
    }

    public bool IsValidPosition(Vector3 ShootPosition, Vector3 Position, Vector3 Normal,LayerMask _LayerMask )
    {
        Vector3 l_StartPosition = transform.position;
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
                    float l_DistanceToHit = Vector3.Distance(m_ValidPoints[i].position, l_RayCastHit.point);
                    if (l_DistanceToHit >= m_MinDistanceToValidPoints && l_DistanceToHit <= m_MaxDistanceToValidPoints)
                    {
                        float l_DotAngle = Vector3.Dot(Normal, l_RayCastHit.normal);
                        if (l_DotAngle < m_MinValidDotAngle)
                        {
                            l_IsValid = false;
                        }
                        //IF HERE, IsValid is true
                        Debug.Log("VALID POSITION " + this.gameObject.name);
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

        if (!l_IsValid)
        {
            //transform.position = l_StartPosition;
        }

        return l_IsValid;
    }
}

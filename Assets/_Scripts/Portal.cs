using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Portal: MonoBehaviour
{
    [Header("References")]
    public Camera m_Camera;
    FPSController m_FPSController;
    public Transform m_OtherPortal;
    public Portal m_MirrorPortal;

    [Space]
    public float m_OffsetNearPlane = 0.1f;
    public List<Transform> m_ValidPoints;
    public float m_MinDistanceToMirrorPortal = 0.5f;
    public float m_MinDistanceToValidPoints;
    public float m_MaxDistanceToValidPoints;
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
            Debug.Log("Teleportable in range"); 
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

    public bool IsValidPosition(Vector3 ShootPosition, Vector3 Position, Vector3 Normal, LayerMask _LayerMask)
    {
        Vector3 l_StartPosition = transform.position;
        Quaternion l_StartRotation = transform.rotation;
        PlacePortal(Position, Normal, ShootPosition);
        gameObject.SetActive(false);
        bool l_IsValid = true;

        if (m_MirrorPortal.isActiveAndEnabled)
        {
            float l_DistanceToMirrorPortal = Vector3.Distance(transform.position, m_MirrorPortal.transform.position);
            l_IsValid = l_DistanceToMirrorPortal >= m_MinDistanceToMirrorPortal;
        }

        for (int i = 0; i < m_ValidPoints.Count; i++)
        {
            Vector3 l_Direction = m_ValidPoints[i].position - ShootPosition;
            float l_Distance = l_Direction.magnitude;
            l_Direction.Normalize();
            Ray l_Ray = new Ray(ShootPosition, l_Direction);
            RaycastHit l_RayCastHit;
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

        if (!l_IsValid)
        {
            transform.position = l_StartPosition;
            transform.rotation = l_StartRotation;
            gameObject.SetActive(true);
        }

        return l_IsValid;
    }

    private void PlacePortal(Vector3 _Position, Vector3 _Normal, Vector3 _ShootPosition)
    {
        transform.position = _Position;
        Vector3 l_ShootDir = _Position - _ShootPosition;
        l_ShootDir.Normalize();
        Quaternion l_DirQuaternion = Quaternion.LookRotation(l_ShootDir);
        transform.rotation = Quaternion.LookRotation(_Normal);
        if (_Normal == Vector3.up || _Normal == Vector3.down)
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, l_DirQuaternion.eulerAngles.y);
    }

    
}

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;


public class Portal : MonoBehaviour, IRestartLevelElement
{
    [Header("References")]
    public Camera m_Camera;
    FPSController m_FPSController;
    public Transform m_OtherPortal;
    public Portal m_MirrorPortal;
    public Renderer m_Renderer;

    [Space]
    public float m_OffsetNearPlane = 0.1f;

    public Collider WallCollider => m_WallCollider;
    private Collider m_WallCollider;
    private List<PortableObject> m_PortableObjects = new();
    private List<PortableObject> m_BannedObjects = new();
    private Collider m_Collider;

    public bool IsPlaced => m_IsPlaced;
    bool m_IsPlaced = false;

    private void Awake()
    {
        m_Collider = GetComponent<Collider>();
    }
    private void Start()
    {
        m_FPSController = GameController.GetGameController().m_Player.GetComponent<FPSController>();
        GameController.GetGameController().AddRestartLevelElement(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        var l_Obj = other.GetComponent<PortableObject>();
        if (l_Obj != null)
        {
            if (l_Obj.CanWarp() && m_MirrorPortal.IsPlaced)
            {
                m_PortableObjects.Add(l_Obj);
                l_Obj.SetIsInPortal(this, m_MirrorPortal, m_WallCollider);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var l_Obj = other.GetComponent<PortableObject>();
        if (m_PortableObjects.Contains(l_Obj))
        {
            m_PortableObjects.Remove(l_Obj);
            l_Obj.ExitPortal(m_WallCollider);
        }
    }


    private void LateUpdate()
    {
        for (int i = 0; i < m_PortableObjects.Count; i++)
        {
            var l_Obj = m_PortableObjects[i];
            Vector3 l_ObjPos = transform.InverseTransformPoint(l_Obj.CenterPos);
            
            if (l_ObjPos.z < 0.0f) // The object went throught the portal
            {
                Vector3 l_ColliderSize = m_Collider.bounds.size;
                if (l_ObjPos.x < l_ColliderSize.x/2 && l_ObjPos.z < l_ColliderSize.z/2) // The object is inside the collider in
                {                                                                       // the horizontal axis
                    l_Obj.Warp();
                }
                else
                {
                    m_PortableObjects.Remove(l_Obj);
                }
            }
        }
        UpdateMirrorPortalCamera();
    }
    private void UpdateMirrorPortalCamera()
    {
        Transform l_PlayerCameraTransform = m_FPSController.Camera.transform;
        Transform l_MirrorCameraTransform = m_MirrorPortal.m_Camera.transform;
        Camera l_MirrorCamera = m_MirrorPortal.m_Camera;

        //PlayerCamera world position to local position from otherPortal;
        Vector3 l_localPosition = m_OtherPortal.InverseTransformPoint(l_PlayerCameraTransform.position);
        //Applies l_localPosition to the MirrorPortal, then transforms that position into WorldSpace;
        Vector3 l_WorldPosition = m_MirrorPortal.transform.TransformPoint(l_localPosition);
        l_MirrorCameraTransform.position = l_WorldPosition;

        Vector3 l_localDirections = m_OtherPortal.InverseTransformDirection(l_PlayerCameraTransform.forward);
        Vector3 l_WorldDirection = m_MirrorPortal.transform.TransformDirection(l_localDirections);
        l_MirrorCameraTransform.forward = l_WorldDirection;

        float l_Distance = Vector3.Distance(l_WorldPosition, m_MirrorPortal.transform.position);
        l_MirrorCamera.nearClipPlane = l_Distance + m_OffsetNearPlane;

        //Vector3 l_EulerCam = l_MirrorCamerasTransform.rotation.eulerAngles;
        //l_MirrorCamerasTransform.rotation = Quaternion.Euler(l_EulerCam.x, l_EulerCam.y, l_EulerCam.z);
    }
    
    public bool IsInHorizontalRotation()
    {
        Vector3 l_Forward = transform.forward;
        l_Forward.x = 0;
        l_Forward.Normalize();

        float l_DotAngle = Vector3.Dot(l_Forward, Vector3.forward);
        return l_DotAngle < 0.01f && l_DotAngle > -0.01f;
    }
    public void PlacePortal(Vector3 _Position, Quaternion _Rotation, Vector3 _LocalScale, Collider _WallCollider)
    {
        transform.position = _Position;
        transform.rotation = _Rotation;
        transform.localScale = _LocalScale;
        m_WallCollider = _WallCollider;
        SetIsPlaced(true);
    }

    public void SetIsPlaced(bool v)
    {
        m_IsPlaced = v;
        gameObject.SetActive(v);
    }

    public void AddBanned(PortableObject portableObject)
    {
        m_BannedObjects.Add(portableObject);
    }

    public void RestartElement()
    {
        gameObject.SetActive(false);
        m_IsPlaced = false;
    }
}

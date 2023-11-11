﻿using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Portal : MonoBehaviour
{
    [Header("References")]
    public Camera m_Camera;
    FPSController m_FPSController;
    public Transform m_OtherPortal;
    public Portal m_MirrorPortal;
    public Renderer m_Renderer;
    public Transform m_Preview;

    [Space]
    public float m_OffsetNearPlane = 0.1f;
    public List<Transform> m_ValidPoints;
    public float m_MinDistanceToMirrorPortal = 0.5f;
    public float m_MinDistanceToValidPoints;
    public float m_MaxDistanceToValidPoints;
    public float m_ValidPointsOffset = 0.1f;
    public float m_MinValidDotAngle = 0.95f;

    public Collider WallCollider => m_WallCollider;
    private Collider m_WallCollider;
    private List<PortableObject> m_PortableObjects = new();
    private List<PortableObject> m_BannedObjects = new();
    private Collider m_Collider;

    public static Action<Portal> OnPlaced;

    public bool IsPlaced => m_IsPlaced;


    bool m_IsPlaced;
    private void Awake()
    {
        m_IsPlaced = false;
        m_Collider = GetComponent<Collider>();
    }
    private void Start()
    {
        m_FPSController = GameController.GetGameController().m_Player.GetComponent<FPSController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var l_Obj = other.GetComponent<PortableObject>();
        if (l_Obj != null)
        {
            Debug.Log("Portal Enter Triggered " + gameObject.name);
            if (l_Obj.CanWarp())
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
            Debug.Log("Portal Exit Triggered " + this.gameObject.name);
            m_PortableObjects.Remove(l_Obj);
            l_Obj.ExitPortal(m_WallCollider);
        }
    }


    private void LateUpdate()
    {
        m_Renderer.enabled = m_MirrorPortal.IsPlaced;

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
        SetPortalTransform(Position, Normal, ShootPosition);
        gameObject.SetActive(false);
        bool l_IsValid = true;

        for (int i = 0; i < m_ValidPoints.Count; i++)
        {
            Vector3 l_Direction = m_ValidPoints[i].position - ShootPosition;
            float l_Distance = l_Direction.magnitude;
            l_Direction.Normalize();
            Ray l_Ray = new Ray(ShootPosition, l_Direction);
            RaycastHit l_RayCastHit;
            //We check if the mirror portal is close to this valid point, just if both portals are facing the same direction
            if (m_MirrorPortal.isActiveAndEnabled && m_MirrorPortal.transform.forward == transform.forward)
            {
                float l_DistanceToMirrorPortal = Vector3.Distance(m_ValidPoints[i].position, m_MirrorPortal.transform.position);
                if (l_DistanceToMirrorPortal < m_MinDistanceToMirrorPortal)
                {
                    l_IsValid = false;
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

    private void SetPortalTransform(Vector3 _Position, Vector3 _Normal, Vector3 _ShootPosition)
    {
        transform.position = _Position;
        Vector3 l_ShootDir = _Position - _ShootPosition;
        l_ShootDir.Normalize();
        Quaternion l_DirQuaternion = Quaternion.LookRotation(l_ShootDir);
        transform.rotation = Quaternion.LookRotation(_Normal);
        if (_Normal == Vector3.up || _Normal == Vector3.down)
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, l_DirQuaternion.eulerAngles.y);
    }

    public bool IsInHorizontalRotation()
    {
        return transform.forward == Vector3.up ||transform.forward == Vector3.down;
    }

    public void SetWallCollider(Collider _Collider)
    {
        m_WallCollider = _Collider;
        Debug.Log(_Collider.gameObject.name);
    }

    public void SetIsPlaced(bool v)
    {
        m_IsPlaced = v;
        gameObject.SetActive(v);
        OnPlaced?.Invoke(this);
    }

    public void AddBanned(PortableObject portableObject)
    {
        m_BannedObjects.Add(portableObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_MinDistanceToMirrorPortal);
    }

    public void SetPreview(bool _Show)
    {

        m_Preview.gameObject.SetActive(_Show);
    }
}

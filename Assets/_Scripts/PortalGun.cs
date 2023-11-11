using System;
using System.Collections.Generic;
using UnityEngine;

public class PortalGun: MonoBehaviour
{
    [Header("References")]
    public Camera m_RayCastCamera;
    public PortalPreview m_BluePreview;
    public PortalPreview m_OrangePreview;

    [Header("Shoot Portals")]
    public Portal m_BluePortal;
    public Portal m_OrangePortal;
    public float m_MaxDistanceToShoot;
    public LayerMask m_ShootableSurface;

    [Header("Attach Objects")]
    public Transform m_AttachingPosition;
    public float m_AttachingSpeed = 2.0f;
    public LayerMask m_AttachObjectLayerMask;
    public float m_MaxDistanceToAttach = 5;
    public float m_ShootAttachedObjectForce = 20.0f;
    bool m_ObjecAlreadyAttached = false;
    Quaternion m_AttachingObjectStartRotation;
    Rigidbody m_ObjectAttached;

    [Header("Input")]
    public KeyCode m_ShootObjectKeyCode = KeyCode.Mouse0;
    public KeyCode m_AttachObjectKeyCode = KeyCode.E;
    public KeyCode m_ShootBluePortalKeyCode = KeyCode.Mouse0;
    public KeyCode m_ShootOrangePortalKeyCode = KeyCode.Mouse1;

    [Header("Debug")]
    public KeyCode m_ToggleShootingActivated = KeyCode.P;
    public bool m_ShootingActivated = false;
    private void Update()
    {
        HandleAttachObjects();
#if UNITY_EDITOR
        if (Input.GetKeyDown(m_ToggleShootingActivated))
            m_ShootingActivated = !m_ShootingActivated;

        if (m_ShootingActivated)
#endif
            HandleShootPortals();

    }

    private void HandleShootPortals()
    {
        m_OrangePreview.SetShow(false);
        m_BluePreview.SetShow(false);

        if (Input.GetKey(m_ShootOrangePortalKeyCode))
            TryPlacePreview(m_OrangePreview);

        else if (Input.GetKey(m_ShootBluePortalKeyCode))
            TryPlacePreview(m_BluePreview);

        if (Input.GetKeyUp(m_ShootOrangePortalKeyCode) && m_OrangePreview.IsValid)
            m_OrangePortal.PlacePortal(m_OrangePreview.transform.position,
                m_OrangePreview.transform.rotation, m_OrangePreview.transform.localScale);

        else if (Input.GetKeyUp(m_ShootBluePortalKeyCode) && m_BluePreview.IsValid)
            m_BluePortal.PlacePortal(m_BluePreview.transform.position,
                m_BluePreview.transform.rotation, m_BluePreview.transform.localScale);
    }

    private void TryPlacePreview(PortalPreview _Preview)
    {
        Ray l_Ray = m_RayCastCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit l_RaycastHit;

        if (Physics.Raycast(l_Ray, out l_RaycastHit, m_MaxDistanceToShoot, m_ShootableSurface.value))
        {
            bool l_IsValidPos = _Preview.IsValidPosition(m_RayCastCamera.transform.position, 
                                        l_RaycastHit.point, l_RaycastHit.normal, m_ShootableSurface.value);
            
            _Preview.SetShow(l_IsValidPos);
        }
    }

    private void HandleAttachObjects()
    {
        if (ObjectBeingAttached())
        {
            UpdateAttachedObject();
            if (Input.GetKeyDown(m_AttachObjectKeyCode) && m_ObjecAlreadyAttached)
                ReleaseAttachedObject(0.0f);
            if (Input.GetKeyDown(m_ShootObjectKeyCode) && m_ObjecAlreadyAttached)
                ReleaseAttachedObject(m_ShootAttachedObjectForce);

        }
        else if (Input.GetKeyDown(m_AttachObjectKeyCode))
            TryAttachObject();
    }

    private void TryAttachObject()
    {
        Ray l_Ray = m_RayCastCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (Physics.Raycast(l_Ray, out RaycastHit l_RayCastHit, m_MaxDistanceToAttach, m_AttachObjectLayerMask.value))
        {
            AttachObject(l_RayCastHit);
        }
    }

    private void AttachObject(RaycastHit _RayCastHit)
    {
        m_ObjectAttached = _RayCastHit.rigidbody;
        m_ObjectAttached.isKinematic = true;
        m_ObjecAlreadyAttached = false;
        //m_ObjectAttached.GetComponent<CompanionCube>().SetAttached(true);
        m_AttachingObjectStartRotation = _RayCastHit.collider.transform.rotation;
    }

    private bool ObjectBeingAttached()
    {
        return m_ObjectAttached != null;
    }

    private void ReleaseAttachedObject(float _Force)
    {
        m_ObjectAttached.transform.SetParent(null);
        m_ObjectAttached.isKinematic = false;
        m_ObjectAttached.velocity = _Force * m_RayCastCamera.transform.forward;
        //m_ObjectAttached.GetComponent<CompanionCube>().SetAttached(false);
        m_ObjectAttached = null;
    }

    private void UpdateAttachedObject()
    {
        Vector3 l_EulerAngles = m_AttachingPosition.rotation.eulerAngles;
        if (!m_ObjecAlreadyAttached)
        {
            Vector3 l_Direction = m_AttachingPosition.transform.position - m_ObjectAttached.transform.position;
            float l_Distance = l_Direction.magnitude;
            float l_Movement = m_AttachingSpeed * Time.deltaTime;
            if (l_Movement >= l_Distance)
            {
                m_ObjectAttached.transform.SetParent(m_AttachingPosition);
                m_ObjecAlreadyAttached = true;
                m_ObjectAttached.MovePosition(m_AttachingPosition.position);
                m_ObjectAttached.MoveRotation(Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z));
            }
            else
            {
                l_Direction /= l_Distance;
                m_ObjectAttached.MovePosition(m_ObjectAttached.transform.position + l_Direction * l_Movement);
                m_ObjectAttached.MoveRotation(Quaternion.Lerp(m_AttachingObjectStartRotation, Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z),
                    1.0f - Mathf.Min(l_Distance / 1.5f, 1.0f)));
            }
        }
        //else
        //{
        //    m_ObjectAttached.MoveRotation(Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z));
        //    m_ObjectAttached.MovePosition(m_AttachingPosition.position);
        //}
    }
}
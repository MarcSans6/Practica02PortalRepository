using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class PortalGun: MonoBehaviour
{
    [Header("References")]
    public Camera m_RayCastCamera;
    public PortalPreview m_BluePreview;
    public PortalPreview m_OrangePreview;
    private AttachGun m_AttachGun;

    [Header("Shoot Portals")]
    public Portal m_BluePortal;
    public Portal m_OrangePortal;
    public float m_MaxDistanceToShoot;
    public LayerMask m_ShootableSurface;
    public float m_ScalingSpeed;
    public float m_MinScale;
    public float m_MaxScale;
    float m_DesiredScale = 1.0f;

    [Header("Input")]
    public KeyCode m_ShootBluePortalKeyCode = KeyCode.Mouse0;
    public KeyCode m_ShootOrangePortalKeyCode = KeyCode.Mouse1;

    [Header("Debug")]
    public KeyCode m_ToggleShootingActivated = KeyCode.P;
    public bool m_ShootingActivated = false;

    private void Awake()
    {
        m_AttachGun = GetComponent<AttachGun>();
    }

    private void Update()
    {
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

        if (m_AttachGun!= null)
        {
            if (m_AttachGun.HasAttachedObject())
            {
                return;
            }
        }

        if (Input.GetKey(m_ShootOrangePortalKeyCode))
        {
            HandleScaleChange();
            TryPlacePreview(m_OrangePreview);
        }

        else if (Input.GetKey(m_ShootBluePortalKeyCode))
        {
            HandleScaleChange();
            TryPlacePreview(m_BluePreview);
        }

        if (Input.GetKeyUp(m_ShootOrangePortalKeyCode) && m_OrangePreview.IsValid)
            m_OrangePortal.PlacePortal(m_OrangePreview.transform.position,
                m_OrangePreview.transform.rotation, m_OrangePreview.transform.localScale, m_OrangePreview.WallCollider);

        else if (Input.GetKeyUp(m_ShootBluePortalKeyCode) && m_BluePreview.IsValid)
            m_BluePortal.PlacePortal(m_BluePreview.transform.position,
                m_BluePreview.transform.rotation, m_BluePreview.transform.localScale, m_BluePreview.WallCollider);
    }

    private void HandleScaleChange()
    {
        float l_ScrollingInput = Input.GetAxis("Mouse ScrollWheel");
        m_DesiredScale += l_ScrollingInput * m_ScalingSpeed *Time.deltaTime;
        m_DesiredScale = Mathf.Clamp(m_DesiredScale, m_MinScale, m_MaxScale);
        Vector3 l_Scale = new Vector3(m_DesiredScale, m_DesiredScale, 1f);
        m_OrangePreview.transform.localScale = l_Scale;
        m_BluePreview.transform.localScale = l_Scale;
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
            if (l_IsValidPos)
            {
                _Preview.SetWallCollider(l_RaycastHit.collider);
            }
            
        }
    }
}

using System;
using UnityEngine;

public class AttachGun: MonoBehaviour
{
    [Header("References")]
    public Transform m_AttachPosition;
    public Camera m_RaycastCam;
    [Header("Settings")]
    public float m_AttachForce;
    public float m_ShootForce;
    public float m_AttachDistance;
    public LayerMask m_LayerMask;
    AttachableObject m_AttachedObj;

    [Header("Input")]
    public KeyCode m_AttachKeyCode = KeyCode.E;
    public KeyCode m_ShootKeyCode = KeyCode.Q;



    private void Update()
    {
        if (m_AttachedObj == null)
        {
            if (Input.GetKeyDown(m_AttachKeyCode))
            {
                TryAttachObject();
            }
        }
        else
        {
            if (Input.GetKeyDown(m_AttachKeyCode))
            {
                m_AttachedObj.Deattach(0.0f, m_RaycastCam.transform.forward);
                m_AttachedObj = null;
            }
            else if (Input.GetKeyDown(m_ShootKeyCode))
            {
                m_AttachedObj.Deattach(m_ShootForce, m_RaycastCam.transform.forward);
                m_AttachedObj = null;
            }
        }
    }

    private void TryAttachObject()
    {
        Ray l_Ray = m_RaycastCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_AttachDistance, m_LayerMask.value))
        {
            AttachableObject l_AttachableObj = l_RaycastHit.transform.GetComponent<AttachableObject>();
            if (l_AttachableObj != null)
            {
                m_AttachedObj = l_AttachableObj;
                m_AttachedObj.Attach(m_AttachPosition, m_AttachForce);
            }
        }
    }

    public bool HasAttachedObject()
    {
        return m_AttachedObj != null;
    }
}

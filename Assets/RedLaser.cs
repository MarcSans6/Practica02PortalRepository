using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedLaser : MonoBehaviour
{
    public float m_MaxDistance = 100;
    public LayerMask m_HitLayerMask;
    LineRenderer m_LineRenderer;
    bool m_ShootingLaser = false;
    public bool m_AutoShooting = false;
    public int ID => m_ID;
    int m_ID;


    private void Awake()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (m_AutoShooting)
        {
            ShootLaser(m_ID);
        }

        if (!m_ShootingLaser)
        {
            gameObject.SetActive(false);
        }
    }

    public void ShootLaser(int _ID)
    {
        if (m_ShootingLaser)
        {
            return;
        }

        m_ID = _ID;
        m_ShootingLaser = true;
        Ray l_Ray = new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward);
        float l_MaxDistance = m_MaxDistance;
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_Ray, out l_RaycastHit, m_MaxDistance, m_HitLayerMask.value))
        {
            l_MaxDistance = Vector3.Distance(m_LineRenderer.transform.position, l_RaycastHit.point);

            IGetLasered l_IGetLasered = l_RaycastHit.transform.GetComponent<IGetLasered>();
            if (l_IGetLasered != null)
            {
                l_IGetLasered.HandleLaserHit(this, l_RaycastHit.point, m_ID);
            }

        }
        m_LineRenderer.SetPosition(1, new Vector3(.0f, .0f, l_MaxDistance));
        m_LineRenderer.gameObject.SetActive(true);

    }
    private void LateUpdate()
    {
        m_ShootingLaser = false;
    }
}

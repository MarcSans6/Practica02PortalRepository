using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Turret : MonoBehaviour, IGetLasered
{
    public float m_DotAlife = .95f;
    public RedLaser m_RedLaser;
    bool m_Alife = true;
    AttachableObject m_AttachableObject;

    private void Awake()
    {
        m_AttachableObject = GetComponent<AttachableObject>();
    }

    public void HandleLaserHit(RedLaser _Laser, Vector3 _HitPos)
    {
        Kill();
    }

    private void Kill()
    {
        m_Alife = false;
    }
        
    private void Update()
    {
        float l_DotAngle = Vector3.Dot(transform.up, Vector3.up);

        if (m_Alife)
        {
            if (l_DotAngle < m_DotAlife && !m_AttachableObject.IsAttached())
            {
                Kill();
            }
        }
        if (m_Alife)
            m_RedLaser.ShootLaser();
    }
}

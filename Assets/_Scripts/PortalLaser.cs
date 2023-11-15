using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class PortalLaser : MonoBehaviour, IGetLasered
{
    public GameObject m_RedLaserPrefab;
    List<RedLaser> m_OutLasersList = new();
    RedLaser m_OutLaser;
    Portal m_Portal;
    CPoolElements m_LaserPoolElements;
    public float m_PortalPosOffset;

    private void Awake()
    {
        m_Portal = GetComponentInParent<Portal>();
        m_LaserPoolElements = new CPoolElements(m_RedLaserPrefab, 10, m_Portal.m_MirrorPortal.transform);
        m_OutLaser = m_LaserPoolElements.GetNextElement().GetComponent<RedLaser>();
    }

    private void Update()
    {
        m_OutLasersList.Clear();
    }

    public void WarpLaser(RedLaser _InLaser, Vector3 _HitPos)
    {
        RedLaser l_OutLaser = m_LaserPoolElements.GetNextElement().GetComponent<RedLaser>();
        Quaternion l_HalfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        
        Vector3 l_RelativePos = m_Portal.transform.InverseTransformPoint(_HitPos);
        l_RelativePos = l_HalfTurn * l_RelativePos;
        Vector3 l_NewPos = m_Portal.m_MirrorPortal.transform.TransformPoint(l_RelativePos);

        Quaternion l_ForwardRot = _InLaser.transform.rotation;

        Quaternion l_RelativeRot = Quaternion.Inverse(m_Portal.transform.rotation) * l_ForwardRot;
        l_RelativeRot = l_HalfTurn * l_RelativeRot;
        Quaternion l_NewRot = m_Portal.m_MirrorPortal.transform.rotation * l_RelativeRot;


        l_OutLaser.transform.rotation = l_NewRot;
        l_OutLaser.transform.position = l_NewPos - l_OutLaser.transform.forward * m_PortalPosOffset; 

        m_OutLasersList.Add(l_OutLaser);
        l_OutLaser.ShootLaser();
    }

    public void HandleLaserHit(RedLaser _Laser, Vector3 _HitPos)
    {
        WarpLaser(_Laser, _HitPos);
    }

    private void LateUpdate()
    {
        foreach (RedLaser l_Laser in m_OutLasersList)
        {
            l_Laser.gameObject.SetActive(true);
        }
    }
}

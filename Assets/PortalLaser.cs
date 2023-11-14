using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class PortalLaser : MonoBehaviour, IGetLasered
{
    public GameObject m_RedLaserPrefab;
    List<RedLaser> m_OutLasersList = new();
    Portal m_Portal;
    CPoolElements m_LaserPoolElements;

    private void Awake()
    {
        m_Portal = GetComponentInParent<Portal>();
        m_LaserPoolElements = new CPoolElements(m_RedLaserPrefab, 10, transform);
    }

    private void Update()
    {
        //m_LaserPoolElements.SetActiveAllElements(false);
        m_OutLasersList.Clear();

    }

    public void WarpLaser(RedLaser _InLaser, Vector3 _HitPos, int _ID)
    {
        RedLaser l_OutLaser = m_LaserPoolElements.GetNextElement().GetComponent<RedLaser>();

        Quaternion l_HalfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        
        Vector3 l_RelativePos = m_Portal.transform.InverseTransformPoint(_HitPos);
        l_RelativePos = l_HalfTurn * l_RelativePos;
        l_OutLaser.transform.position = m_Portal.m_MirrorPortal.transform.TransformPoint(l_RelativePos);

        Quaternion l_RelativeRot = Quaternion.Inverse(m_Portal.transform.rotation) * _InLaser.transform.rotation;
        l_RelativeRot = l_HalfTurn * l_RelativeRot;
        l_OutLaser.transform.rotation = m_Portal.m_MirrorPortal.transform.rotation * l_RelativeRot;

        m_OutLasersList.Add(l_OutLaser);
        l_OutLaser.ShootLaser(_ID);
    }

    private void CreateNewLaser(RedLaser _InLaser, Vector3 _HitPos)
    {
        Quaternion l_HalfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);

        RedLaser l_OutLaser = Instantiate(_InLaser);

        Vector3 l_RelativePos = m_Portal.transform.InverseTransformPoint(_HitPos);
        l_RelativePos = l_HalfTurn * l_RelativePos;
        l_OutLaser.transform.position = m_Portal.m_MirrorPortal.transform.TransformPoint(l_RelativePos);

        Quaternion l_RelativeRot = Quaternion.Inverse(m_Portal.transform.rotation) * l_OutLaser.transform.rotation;
        l_RelativeRot = l_HalfTurn * l_RelativeRot;
        l_OutLaser.transform.rotation = m_Portal.m_MirrorPortal.transform.rotation * l_RelativeRot;
    }

    public void HandleLaserHit(RedLaser _Laser, Vector3 _HitPos, int _ID)
    {
        WarpLaser(_Laser, _HitPos, _ID);
    }

    private void LateUpdate()
    {
        foreach (RedLaser l_Laser in m_OutLasersList)
        {
            l_Laser.gameObject.SetActive(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class RefractionCube : MonoBehaviour, IGetLasered
{
    [Header("References")]
    public RedLaser m_RedLaser;
    [Space]
    bool m_Reflected = false;

    public void HandleLaserHit(RedLaser _Laser, Vector3 _HitPos)
    {
        Reflect();
    }

    public void LateUpdate()
    {
        m_Reflected = false;
    }



    public void Reflect()
    {
        if (m_Reflected)
        {
            return;
        }

        m_Reflected = true;
        m_RedLaser.ShootLaser();
    }

}

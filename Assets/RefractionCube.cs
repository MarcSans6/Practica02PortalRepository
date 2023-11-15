using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class RefractionCube : MonoBehaviour, IGetLasered, IRestartLevelElement
{
    [Header("References")]
    public RedLaser m_RedLaser;
    [Space]
    bool m_Reflected = false;

    Rigidbody m_Rigidbody;

    Vector3 m_StartPos;
    Quaternion m_StartRot;
    Vector3 m_StartScale;
    Vector3 m_StartVelocity;
    Vector3 m_StartAngularVel;



    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        m_StartPos = transform.position;
        m_StartRot = transform.rotation;
        m_StartScale = transform.localScale;
        m_StartVelocity = m_Rigidbody.velocity;
        m_StartAngularVel = m_Rigidbody.angularVelocity;
    }

    void Start()
    {
        GameController.GetGameController().AddRestartLevelElement(this);
    }


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

    public void RestartElement()
    {
        transform.position = m_StartPos;
        transform.rotation = m_StartRot;
        transform.localScale = m_StartScale;
        m_Rigidbody.velocity = m_StartVelocity;
        m_Rigidbody.angularVelocity = m_StartAngularVel;

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Turret : MonoBehaviour, IGetLasered, IRestartLevelElement
{
    public float m_DotAlife = .95f;
    public RedLaser m_RedLaser;
    bool m_Alife = true;
    AttachableObject m_AttachableObject;

    Rigidbody m_Rigidbody;

    Vector3 m_StartPos;
    Quaternion m_StartRot;
    Vector3 m_StartScale;
    Vector3 m_StartVelocity;
    Vector3 m_StartAngularVel;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CompanionCube>())
        {
            Kill();
        }
    }

    private void Awake()
    {
        m_AttachableObject = GetComponent<AttachableObject>();
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
        //Kill();
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

    public void RestartElement()
    {
        transform.position = m_StartPos;
        transform.rotation = m_StartRot;
        transform.localScale = m_StartScale;
        m_Rigidbody.velocity = m_StartVelocity;
        m_Rigidbody.angularVelocity = m_StartAngularVel;

        m_Alife = true;

    }
}

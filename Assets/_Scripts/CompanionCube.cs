using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Analytics;

[RequireComponent(typeof(Rigidbody))]
public class CompanionCube : MonoBehaviour, IRestartLevelElement
{
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

    public void RestartElement()
    {
        transform.position = m_StartPos;
        transform.rotation = m_StartRot;
        transform.localScale = m_StartScale;
        m_Rigidbody.velocity = m_StartVelocity;
        m_Rigidbody.angularVelocity = m_StartAngularVel;
    }
}

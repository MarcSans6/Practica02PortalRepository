using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingWall : MonoBehaviour
{
    [Min(1)]
    public float m_BounceMultiplier = 3;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody l_Rigidbody = other.GetComponent<Rigidbody>(); ;

        if (l_Rigidbody != null)
        {
            ApplyBounceEffect(l_Rigidbody, transform.forward);
        }
    }
    private void ApplyBounceEffect(Rigidbody l_Rigidbody, Vector3 _Normal)
    {
        float l_StartSpeed = l_Rigidbody.velocity.magnitude;
        Vector3 l_StartDir = l_Rigidbody.velocity.normalized;

        Debug.Log(l_StartSpeed);

        Vector3 l_ReflectedDir = Vector3.Reflect(l_StartDir, _Normal);
        float l_Force = GetBounceForce(l_Rigidbody, l_StartSpeed);

        Debug.Log(l_Force);


        l_Rigidbody.velocity = Vector3.zero;
        l_Rigidbody.AddForce(l_ReflectedDir * l_Force);
    }
    private float GetBounceForce(Rigidbody l_Rigidbody, float l_StartSpeed)
    {
        float l_DesiredSpeed = l_StartSpeed * m_BounceMultiplier;
        float l_Force = l_Rigidbody.mass * (l_DesiredSpeed / Time.fixedDeltaTime);
        return l_Force;
    }
}

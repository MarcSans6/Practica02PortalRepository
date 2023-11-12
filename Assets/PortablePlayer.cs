using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FPSController))]
public class PortablePlayer : PortableObject
{
    public Action OnWarp;
    FPSController m_FPSController;
    protected override void Awake()
    {
        base.Awake();
        m_FPSController = GetComponent<FPSController>();
    }
    void Start()
    {
        //m_DesiredRotation = transform.rotation;
    }

    public override void Warp()
    {
        Vector3 l_EulerAngles = transform.rotation.eulerAngles;
        base.Warp();
        OnWarp?.Invoke();
        Debug.Break();

        Vector3 l_EulerAnglesWarp = transform.rotation.eulerAngles;

        l_EulerAnglesWarp.x = l_EulerAngles.x;

        transform.rotation = Quaternion.Euler(l_EulerAnglesWarp);
        //Adjust Yaw
        Vector3 l_Forward = m_FPSController.YawController.forward;
        float l_Yaw = Mathf.Atan2(l_Forward.x, l_Forward.z) * Mathf.Rad2Deg;
        m_FPSController.SetYaw(l_Yaw);
    }
}

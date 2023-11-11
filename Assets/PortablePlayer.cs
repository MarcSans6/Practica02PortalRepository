using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FPSController))]
public class PortablePlayer : PortableObject
{
    FPSController m_FPSController;
    Quaternion m_DesiredRotation;
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
        base.Warp();

        //Adjust Yaw
        Vector3 l_Forward = transform.forward;
        l_Forward.y = 0;
        l_Forward.Normalize();
        float l_Yaw = Mathf.Atan2(l_Forward.x, l_Forward.z) * Mathf.Rad2Deg;
        m_FPSController.SetYaw(l_Yaw);
    }
}

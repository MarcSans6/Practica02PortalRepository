﻿using UnityEngine;

public class TeleportablePlayer: Teleportable
{
    FPSController m_FPSController;
    protected override void Awake()
    {
        base.Awake();
        m_FPSController = GetComponent<FPSController>();
    }

    public override bool CanTeleport(Portal _Portal)
    {
        return base.CanTeleport(_Portal);
    }

    public override void Teleport(Portal _Portal)
    {
        base.Teleport(_Portal);

        Vector3 l_Forward = transform.forward;
        l_Forward.y = 0;
        l_Forward.Normalize();
        Vector3 l_LocalForward = _Portal.m_OtherPortal.InverseTransformDirection(l_Forward);
        Vector3 l_WorldForward = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalForward);
        float l_Yaw = Mathf.Atan2(l_WorldForward.x, l_WorldForward.z) * Mathf.Rad2Deg;
        m_FPSController.SetYaw(l_Yaw);
    }
}


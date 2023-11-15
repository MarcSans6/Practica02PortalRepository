using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

    protected override void AfterWarp(Transform _InTransform, Transform _OutTransform)
    {
        base.AfterWarp(_InTransform, _OutTransform);
        OnWarp?.Invoke();
        Debug.Break();
        SetRotationForward(_InTransform, _OutTransform);
    }
    private void SetRotationForward(Transform _InTransform, Transform _OutTransform)
    {
        Quaternion l_ForwardRotation = m_FPSController.RollController.rotation;

        Quaternion l_RelativeRot = Quaternion.Inverse(_InTransform.rotation) * l_ForwardRotation;
        l_RelativeRot = m_HalfTurn * l_RelativeRot;
        Quaternion l_NewRot = _OutTransform.rotation * l_RelativeRot;

        Vector3 l_NewEuler = l_NewRot.eulerAngles;

        float l_Yaw = l_NewEuler.y;
        float l_Pitch = l_NewEuler.x;
        //float l_Roll = l_NewEuler.z;

        if (l_Pitch > 90)
        {
            l_Pitch -= 360;
        }

        m_FPSController.SetYaw(l_Yaw);
        m_FPSController.SetPitch(l_Pitch);
        //m_FPSController.SetRoll(l_Roll);
    }
}

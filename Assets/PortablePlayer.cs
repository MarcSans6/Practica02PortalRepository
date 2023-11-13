using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FPSController))]
public class PortablePlayer : PortableObject
{
    public Action OnWarp;
    FPSController m_FPSController;

    private Vector3 m_WarpEulerOffset = Vector3.zero;


    protected override void Awake()
    {
        base.Awake();
        m_FPSController = GetComponent<FPSController>();
    }
    void Start()
    {
        //m_DesiredRotation = transform.rotation;
    }

    protected override void AfterWarp(Transform _InTransform, Transform _OutTransform)
    {
        base.AfterWarp(_InTransform, _OutTransform);
        OnWarp?.Invoke();

        //FisrtMethod(_InTransform, _OutTransform);
    }

    private void FisrtMethod(Transform _InTransform, Transform _OutTransform)
    {
        SetWarpAngleOffset(_InTransform, _OutTransform);

        ChangeYaw();
        ChangePitch();
        //ChangeRoll();
    }

    private void SetWarpAngleOffset(Transform _InTransform, Transform _OutTransform)
    {
        Vector3 l_InPortalEuler = _InTransform.rotation.eulerAngles;
        Vector3 l_OuPortalEuler = _OutTransform.rotation.eulerAngles;

        m_WarpEulerOffset.x = l_OuPortalEuler.x - l_InPortalEuler.x;
        m_WarpEulerOffset.y = l_OuPortalEuler.y - l_InPortalEuler.y;
        m_WarpEulerOffset.z = l_OuPortalEuler.z - l_InPortalEuler.z;

        Debug.Log(m_WarpEulerOffset);
    }
    private void ChangeYaw()
    {
        float l_CurrentYaw = m_FPSController.YawController.eulerAngles.y;

        float l_Yaw = l_CurrentYaw + m_WarpEulerOffset.y + 180.0f;
        m_FPSController.SetYaw(l_Yaw);
    }
    private void ChangePitch()
    {
        float l_CurrentPitch = m_FPSController.PitchController.eulerAngles.x;
        float l_Pitch = l_CurrentPitch + m_WarpEulerOffset.x;
        m_FPSController.SetPitch(l_Pitch);
    }

    private void ChangeRoll()
    {
        float l_CurrentRoll = m_FPSController.RollController.eulerAngles.z;
        float l_Roll = l_CurrentRoll + m_WarpEulerOffset.z;
        m_FPSController.SetRoll(l_Roll);
    }
}

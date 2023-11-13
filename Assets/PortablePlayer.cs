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

        SetWarpAngleOffset(_InTransform, _OutTransform);

        ChangeYaw();
        ChangePitch();
    }
    private void SetWarpAngleOffset(Transform _InTransform, Transform _OutTransform)
    {
        Vector3 l_InPortalEuler = _InTransform.rotation.eulerAngles;
        Vector3 l_OuPortalEuler = _OutTransform.rotation.eulerAngles;

        m_WarpEulerOffset.x = l_OuPortalEuler.x - l_InPortalEuler.x;
        m_WarpEulerOffset.y = l_OuPortalEuler.y - l_InPortalEuler.y;
        m_WarpEulerOffset.z = l_OuPortalEuler.z - l_InPortalEuler.z;
    }

    private void ChangeYaw2(Transform _InTransform, Transform _OutTransform)
    {
        Vector3 l_InPortalForward = _InTransform.forward;
        Vector3 l_OutPortalForward = _OutTransform.forward;

        Quaternion l_Rot = Quaternion.FromToRotation(l_OutPortalForward, l_InPortalForward);

        Vector3 l_RotEuler = l_Rot.eulerAngles;

        Debug.Log("X rot: " + l_RotEuler.x);
        Debug.Log("Y rot: " + l_RotEuler.y);
        Debug.Log("Z rot: " + l_RotEuler.z);

        //Quaternion l_YRotOffset = Quaternion.Euler(0.0f, l_YAngle, 0.0f);

        //Vector3 l_YawForwardRotated = l_YRotOffset * m_FPSController.YawController.forward;

        //float l_Yaw = Quaternion.LookRotation(l_YawForwardRotated).eulerAngles.y;

        //m_FPSController.SetYaw(l_Yaw);


        //float l_Yaw = Mathf.Atan2(l_YawForward.x, l_YawForward.z) * Mathf.Rad2Deg;
        //m_FPSController.SetYaw(l_Yaw);

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
        float l_Pitch = l_CurrentPitch + m_WarpEulerOffset.x + 180.0f;
        Debug.Log(l_Pitch);
        m_FPSController.SetPitch(l_Pitch);
    }
}

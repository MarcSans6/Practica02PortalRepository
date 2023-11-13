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

    protected override void AfterWarp(Transform _InTransform, Transform _OutTransform)
    {
        base.AfterWarp(_InTransform, _OutTransform);
        OnWarp?.Invoke();

        Vector3 l_CurrentInPortalEuler = _InTransform.rotation.eulerAngles;
        Vector3 l_CurrentOutPortalEuler = _OutTransform.rotation.eulerAngles;

        ChangeYaw(_InTransform, _OutTransform);

        _InTransform.rotation = Quaternion.Euler(l_CurrentInPortalEuler);
        _OutTransform.rotation = Quaternion.Euler(l_CurrentOutPortalEuler);

        //Quaternion l_PortalsRelativeRot = Quaternion.FromToRotation(_InTransform.forward, _OutTransform.forward);

        ////Adjust Yaw
        //Vector3 l_YawForward = m_FPSController.YawController.forward;
        //l_YawForward.Normalize();
        ////Vector3 l_LocalYawForward = m_InPortal.m_OtherPortal.InverseTransformDirection(l_YawForward);
        ////Vector3 l_WorldYawForward = m_OutPortal.transform.TransformDirection(l_LocalYawForward);

        //l_YawForward = l_PortalsRelativeRot * l_YawForward;

        //Quaternion l_TempQuat = Quaternion.LookRotation(l_YawForward);


        //float l_Yaw = l_TempQuat.eulerAngles.y;
        //m_FPSController.SetYaw(l_Yaw);

        //////Adjust Pitch
        ////Vector3 l_PitchForward = m_FPSController.PitchController.forward;
        ////l_YawForward.Normalize();
        ////Vector3 l_LocalPitchForward = m_InPortal.m_OtherPortal.InverseTransformDirection(l_PitchForward);
        ////Vector3 l_WorldPitchForward = m_OutPortal.transform.TransformDirection(l_LocalPitchForward);

        ////Quaternion l_Temp = Quaternion.LookRotation(l_WorldPitchForward);


        //float l_Pitch = l_Temp.eulerAngles.x;

        //Debug.Log(l_Pitch);

        ////m_FPSController.SetPitch(l_Pitch);
    }

    private void ChangeYaw(Transform _InTransform, Transform _OutTransform)
    {
        //We set X rot to 0 in both portals
        _InTransform.rotation = Quaternion.Euler(new Vector3(0.0f, _InTransform.rotation.y, _InTransform.rotation.z));
        _OutTransform.rotation = Quaternion.Euler(new Vector3(0.0f, _OutTransform.rotation.y, _OutTransform.rotation.z));

        //We calculate the new Yaw
        Vector3 l_YawForward = m_FPSController.YawController.forward;
        Vector3 l_RelativeYaw = _InTransform.InverseTransformDirection(l_YawForward);
        l_RelativeYaw = m_HalfTurn * l_RelativeYaw;
        l_YawForward = _OutTransform.TransformDirection(l_RelativeYaw);
        float l_Yaw = Mathf.Atan2(l_YawForward.x, l_YawForward.z) * Mathf.Rad2Deg;
        m_FPSController.SetYaw(l_Yaw);
    }
}

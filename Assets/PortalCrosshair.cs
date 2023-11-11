using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCrosshair : MonoBehaviour
{
    [SerializeField] Portal m_BluePortal;
    [SerializeField] Portal m_OrangePortal;

    [SerializeField] GameObject m_DefaultCrosshair;
    [SerializeField] GameObject m_BlueCrosshair;
    [SerializeField] GameObject m_OrangeCrosshair;

    private void Start()
    {
        m_DefaultCrosshair.SetActive(true);
    }
    private void Update()
    {
        CheckActivePortals();
    }   

    private void CheckActivePortals()
    {
      
        m_BlueCrosshair.SetActive(m_BluePortal.IsPlaced);
        m_OrangeCrosshair.SetActive(m_OrangePortal.IsPlaced);
       
    }

}

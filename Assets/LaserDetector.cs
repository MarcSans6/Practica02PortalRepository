using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LaserDetector : MonoBehaviour, IGetLasered
{
    public UnityEvent m_OnLasered;
    public Material m_LaseredMaterial;
    public Material m_UnlaseredMaterial;
    public MeshRenderer m_CenterMesh;

    bool m_Lasered = false;

    public void HandleLaserHit(RedLaser _Laser, Vector3 _HitPos)
    {
        Activate();
    }

    private void Update()
    {
        if (!m_Lasered)
        {
            m_CenterMesh.material = m_UnlaseredMaterial;
        }
    }

    private void Activate()
    {
        if (m_Lasered)
        {
            return;
        }
        m_CenterMesh.material = m_LaseredMaterial;
        m_Lasered = true;
        if (true)
        {
            m_OnLasered?.Invoke();
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        m_Lasered = false;
    }
}

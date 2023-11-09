using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class RefractionCube : MonoBehaviour
{
    public LineRenderer m_LineRenderer;
    public float m_MaxLaserDistance;
    bool m_Reflected = false;
    public LayerMask m_LayerLayerMask;
    private void Awake()
    {
        m_LineRenderer.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (!m_Reflected)
        {
            m_LineRenderer.gameObject.SetActive(false);
        }
    }

    public void LateUpdate()
    {
        m_Reflected = false;
    }

    public void Reflect()
    {
        if (m_Reflected)
        {
            return;
        }

        m_Reflected = true;
        Ray l_Ray = new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward);
        float l_MaxDistance = m_MaxLaserDistance;
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_Ray, out l_RaycastHit, m_MaxLaserDistance, m_LayerLayerMask.value))
        {
            l_MaxDistance = Vector3.Distance(m_LineRenderer.transform.position, l_RaycastHit.point);

            var l_RefractionCube = l_RaycastHit.transform.GetComponent<RefractionCube>();
            if (l_RefractionCube != null)
            {
                l_RefractionCube.Reflect();
            }
        }
        m_LineRenderer.SetPosition(1, new Vector3(.0f, .0f, l_MaxDistance));
        m_LineRenderer.gameObject.SetActive(true);
    }

}

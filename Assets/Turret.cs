using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float m_DotAlife = .95f;
    public LineRenderer m_LineRenderer;
    public float m_MaxLaserDistance;
    public LayerMask m_LayerLayerMask;

    private void Update()
    {
        Ray l_Ray = new(m_LineRenderer.transform.position, m_LineRenderer.transform.forward);
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

            if (l_RaycastHit.transform == GameController.GetGameController().m_Player.transform)
            {
                GameController.GetGameController().m_Player.KillPlayer();
            }

        }
        m_LineRenderer.SetPosition(1, new Vector3(.0f, .0f, l_MaxDistance));
        float l_DotAngle = Vector3.Dot(transform.up, Vector3.up);
        bool l_Alife = l_DotAngle >= m_DotAlife;
        m_LineRenderer.gameObject.SetActive(l_Alife);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class DestroyingSurface : MonoBehaviour
{
    public LayerMask m_DestroyableLayerMask;

    private void OnCollisionEnter(Collision collision)
    {
        if (m_DestroyableLayerMask == (m_DestroyableLayerMask | (1 << collision.gameObject.layer)))
        {
            collision.gameObject.SetActive(false);
        }
    }
}

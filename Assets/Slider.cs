using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slider : MonoBehaviour
{
    float m_PlayerDrag;

    private void OnCollisionEnter(Collision collision)
    {
        ISlide l_ISlide = collision.gameObject.GetComponent<ISlide>();
        if (l_ISlide != null)
        {
            l_ISlide.OnEnterSlider();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        ISlide l_ISlide = collision.gameObject.GetComponent<ISlide>();
        if (l_ISlide != null)
        {
            l_ISlide.OnExitSlider();
        }
    }
}

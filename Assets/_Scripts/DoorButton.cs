using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    [SerializeField]
    AnimationClip m_DoorClosedAnim;
    [SerializeField]
    AnimationClip m_DoorOpenedAnim;
    [SerializeField]
    Animation m_AnimationComp;
    public float m_OpeningSpeed;
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<CompanionCube>() != null)
        m_AnimationComp.CrossFade(m_DoorOpenedAnim.name, m_OpeningSpeed);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CompanionCube>() != null)
            m_AnimationComp.CrossFade(m_DoorClosedAnim.name, m_OpeningSpeed );
    }
}

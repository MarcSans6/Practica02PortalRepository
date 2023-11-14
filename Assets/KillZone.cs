using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    [SerializeField] BoxCollider m_KillZone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == GameController.GetGameController().m_Player.transform)
        {
            GameController.GetGameController().m_Player.KillPlayer();
        }
    }
}

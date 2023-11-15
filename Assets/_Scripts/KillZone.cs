using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == GameController.GetGameController().m_Player.transform)
        {
            GameController.GetGameController().m_Player.KillPlayer();
        }
    }
}

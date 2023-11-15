using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform m_SpawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FPSController>())
        {
            GameController.GetGameController().m_CurrentPlayerSpawnPosition = m_SpawnPoint.position;
        }
    }
}

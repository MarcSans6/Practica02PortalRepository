using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField]
    Transform m_SpawnPoint;
    [SerializeField]
    GameObject m_SpawnCube;

    KeyCode m_InteractKeyCode = KeyCode.E;

    bool m_PlayerInSpawnZone;

    GameObject m_InstanceCube;

    private void Awake()
    {
    }

    void Update()
    {
        if (m_PlayerInSpawnZone && Input.GetKeyDown(m_InteractKeyCode))
        {
            SpawnCube();
        }   
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == GameController.GetGameController().m_Player.transform)
        {
            m_PlayerInSpawnZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == GameController.GetGameController().m_Player.transform)
        {
            m_PlayerInSpawnZone = false;
        }
    }

    private void SpawnCube()
    {
        if (m_InstanceCube != null)
        {
            IRestartLevelElement l_CubeRestart = m_InstanceCube.GetComponent<IRestartLevelElement>();
            GameController.GetGameController().RemoveRestartElement(l_CubeRestart);
            Destroy(m_InstanceCube);
            m_InstanceCube = null;
        }
        m_InstanceCube = Instantiate(m_SpawnCube);
        m_InstanceCube.transform.position = m_SpawnPoint.transform.position;
    }

}

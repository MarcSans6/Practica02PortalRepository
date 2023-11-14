using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IRestartLevelElement
{
    public Camera m_Camera;
    [SerializeField]
    GameObject m_GameOverScreen;
    KeyCode m_RestartLevelKey = KeyCode.Return;

    public void RestartElement()
    {
        transform.position = GameController.GetGameController().m_PlayerSpawnPosition;
        transform.rotation = GameController.GetGameController().m_PlayerSpawnRotation;

        m_GameOverScreen.SetActive(false);


    }

    private void Awake()
    {

        if (GameController.GetGameController().m_Player==null)
        {
            GameController.GetGameController().m_Player = this;
            DontDestroyOnLoad(gameObject);
            GameController.GetGameController().AddRestartLevelElement(this);

        }
        else
        {
            GameObject.Destroy(gameObject);
            GameController.GetGameController().m_Player.transform.position = transform.position;
            GameController.GetGameController().m_Player.transform.rotation = transform.rotation;
        }

        GameController.GetGameController().m_PlayerSpawnPosition = transform.position;
        GameController.GetGameController().m_PlayerSpawnRotation = transform.rotation;


    }

    public void KillPlayer()
    {
        m_GameOverScreen.SetActive(true);
        GameController.GetGameController().OnGameOver();
    }



}

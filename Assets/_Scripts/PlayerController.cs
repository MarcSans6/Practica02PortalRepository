using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Camera m_Camera;

    public void RestartElement()
    {
        CharacterController l_CharacterController = GetComponent<CharacterController>();
        l_CharacterController.enabled = false;
        transform.position = GameController.GetGameController().m_PlayerSpawnPosition;
        transform.rotation = GameController.GetGameController().m_PlayerSpawnRotation;
        l_CharacterController.enabled = true;
    }

    private void Awake()
    {

        if (GameController.GetGameController().m_Player==null)
        {
            GameController.GetGameController().m_Player = this;
            DontDestroyOnLoad(gameObject);
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



}

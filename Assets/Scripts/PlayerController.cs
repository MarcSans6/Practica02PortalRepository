using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Animation m_FadeImageAnim;
    [SerializeField] AnimationClip m_FadeInAnimation;
    [SerializeField] AnimationClip m_FadeOutAnimation;
    public Camera m_Camera;

    public void RestartElement()
    {
        CharacterController l_CharacterController = GetComponent<CharacterController>();
        l_CharacterController.enabled = false;
        transform.position = GameManager.GetGameManager().m_PlayerSpawnPosition;
        transform.rotation = GameManager.GetGameManager().m_PlayerSpawnRotation;
        l_CharacterController.enabled = true;
    }

    private void Awake()
    {

        if (GameManager.GetGameManager().m_Player==null)
        {
            GameManager.GetGameManager().m_Player = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            GameObject.Destroy(gameObject);
            GameManager.GetGameManager().m_Player.transform.position = transform.position;
            GameManager.GetGameManager().m_Player.transform.rotation = transform.rotation;
        }

        GameManager.GetGameManager().m_PlayerSpawnPosition = transform.position;
        GameManager.GetGameManager().m_PlayerSpawnRotation = transform.rotation;


    }



}

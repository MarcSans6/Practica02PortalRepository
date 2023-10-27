using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager m_GameManager;
    public GameObject m_DestroyObjects;
    public Vector3 m_PlayerSpawnPosition;
    public Quaternion m_PlayerSpawnRotation;
    public PlayerController m_Player;

    readonly string m_Level2SceneName = "Level2Scene";

    bool m_GameOver = false;


    private void Start()
    {
    }
    public static GameManager GetGameManager()
    {
        if (m_GameManager == null)
        {
            GameObject l_gameObject = new GameObject("GameManager");
            m_GameManager = l_gameObject.AddComponent<GameManager>();
            DontDestroyOnLoad(l_gameObject);

            m_GameManager.m_DestroyObjects = new GameObject("DestroyObjects");
            m_GameManager.m_DestroyObjects.transform.SetParent(m_GameManager.transform);

        }
        return m_GameManager;
    }

    private void DestroyObjects()
    {
        foreach (Transform t in m_DestroyObjects.GetComponentInChildren<Transform>())
        {
            if (t != m_DestroyObjects.transform)
            {
                Destroy(t.gameObject);
            }
        }
    }

    private void Update()
    {
//#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.N))
            EnterNextLevel();
        if (Input.GetKeyDown(KeyCode.L))
//#endif
        if (m_GameOver && Input.GetKeyDown(KeyCode.Return))
        {
        }
    }

    public void EnterNextLevel()
    {
    }

    private void LoadLevel2Scene()
    {
        SceneManager.LoadScene(m_Level2SceneName);
    }

    public void OnGameOver()
    {
        m_GameOver = true;
    }
    
    

    #region RestartLevel
    }
    #endregion

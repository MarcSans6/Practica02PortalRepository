using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    static GameController m_GameController;
    public GameObject m_DestroyObjects;
    public Vector3 m_PlayerSpawnPosition;
    public Quaternion m_PlayerSpawnRotation;
    public PlayerController m_Player;
    public List<IRestartLevelElement> m_IRestartElementList = new List<IRestartLevelElement>();
    

    bool m_GameOver = false;

    private void Start()
    {
    }
    public static GameController GetGameController()
    {
        if (m_GameController == null)
        {
            GameObject l_gameObject = new GameObject("GameController");
            m_GameController = l_gameObject.AddComponent<GameController>();
            DontDestroyOnLoad(l_gameObject);

            m_GameController.m_DestroyObjects = new GameObject("DestroyObjects");
            m_GameController.m_DestroyObjects.transform.SetParent(m_GameController.transform);

        }
        return m_GameController;
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
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L))
            RestartLevel();
#endif
        if (m_GameOver && Input.GetKeyDown(KeyCode.Return))
            RestartLevel();
    }

    public void AddRestartLevelElement(IRestartLevelElement _Element)
    {
        if (_Element == null || m_IRestartElementList.Contains(_Element)) return;
        m_IRestartElementList.Add(_Element);
    }

    public void RemoveRestartElement(IRestartLevelElement _Element)
    {
        if (_Element == null || m_IRestartElementList.Contains(_Element)) return;
        m_IRestartElementList.Remove(_Element);
    }

    public void OnGameOver()
    {
        m_GameOver = true;
    }
    
    #region RestartLevel
    public void RestartLevel()
    {
        DestroyObjects();
        foreach (IRestartLevelElement element in m_IRestartElementList)
        {
            element?.RestartElement();
        }

    }   
    #endregion
}

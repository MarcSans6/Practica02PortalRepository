using System.Runtime;
using UnityEngine;

[RequireComponent(typeof(FPSController))]
public class PlayerController : MonoBehaviour, IRestartLevelElement, IGetLasered
{
    public Camera m_Camera;
    [SerializeField]
    GameObject m_GameOverScreen;
    public GameObject m_WinScreen;

    FPSController m_FPSController;

    public void RestartElement()
    {
        m_FPSController.enabled = true;
        transform.position = GameController.GetGameController().m_CurrentPlayerSpawnPosition;
        m_GameOverScreen.SetActive(false);
        m_WinScreen.SetActive(false);
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
        }

        GameController.GetGameController().m_PlayerStartSpawnPosition = transform.position;
        GameController.GetGameController().m_CurrentPlayerSpawnPosition = transform.position;

    }

    private void Start()
    {
        m_FPSController = GetComponent<FPSController>();
    }

    public void KillPlayer()
    {
        m_GameOverScreen.SetActive(true);
        GameController.GetGameController().OnGameOver();
        m_FPSController.enabled = false;

    }

    public void PlayerWins()
    {
        m_WinScreen.SetActive(true);
        GameController.GetGameController().OnWinGame();
        m_FPSController.enabled = false;
    }

    public void HandleLaserHit(RedLaser _Laser, Vector3 _HitPos)
    {
        KillPlayer();
    }
}

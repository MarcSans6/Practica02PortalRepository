using System.Runtime;
using UnityEngine;

[RequireComponent(typeof(FPSController))]
public class PlayerController : MonoBehaviour, IRestartLevelElement, IGetLasered
{
    public Camera m_Camera;
    [SerializeField]
    GameObject m_GameOverScreen;

    FPSController m_FPSController;

    public void RestartElement()
    {
        m_FPSController.enabled = true;
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

    public void HandleLaserHit(RedLaser _Laser, Vector3 _HitPos, int _ID)
    {
        KillPlayer();
    }
}

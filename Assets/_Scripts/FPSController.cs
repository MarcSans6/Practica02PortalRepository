using System;
using System.Collections;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    public bool CamRotating { get; private set; }
    public bool Moving { get; private set; }
    public bool Sprinting { get; private set; }
    public bool Jumping { get; private set; }

    [Header("Aim")]
    [SerializeField] float m_YawSpeed;
    [SerializeField] float m_PitchSpeed;
    [Space]
    [SerializeField] float m_MaxPitch;
    [SerializeField] float m_MinPitch;
    [Space]
    [SerializeField] bool m_YawInverted;
    [SerializeField] bool m_PitchInverted;
    [SerializeField] Transform m_PitchController;
    float m_Yaw;
    float m_Pitch;

    [Header("Shoot")]
    [SerializeField] bool m_AutoReload;
    public bool AutoReload => m_AutoReload;


    [Header("Movement")]
    [SerializeField] float m_WalkingSpeed;
    [SerializeField] float m_SprintSpeed;
    [SerializeField] float m_CrouchingSpeed;
    [SerializeField] float m_JumpSpeed;
    [SerializeField] float m_CoyoteTime;
    float m_LastTimeOnGround;
    float m_VerticalSpeed;

    [Header("Input")]
    [SerializeField] KeyCode m_UpKeyCode = KeyCode.W;
    [SerializeField] KeyCode m_LeftKeyCode = KeyCode.A;
    [SerializeField] KeyCode m_DownKeyCode = KeyCode.S;
    [SerializeField] KeyCode m_RightKeyCode = KeyCode.D;
    [SerializeField] KeyCode m_JumpKeyCode = KeyCode.Space;
    [SerializeField] KeyCode m_SprintKeyCode = KeyCode.LeftShift;
    //[SerializeField] KeyCode m_CrouchKeyCode = KeyCode.LeftControl;
    [Space]
    [SerializeField] Camera m_Camera;

    [Header("Animatiosn")]
    [SerializeField] Animation m_PitchCtrlAnimation;
    [SerializeField] AnimationClip m_CameraShakeAnim;
    [SerializeField] Animator m_PlayerCameraAnimator;
 
    [Header("Debug")]
    [SerializeField] bool m_AngleLocked;
    [Space]
    [SerializeField] KeyCode m_DebugLockAngleKeyCode = KeyCode.O;
    [SerializeField] KeyCode m_DebugLockKeyCode = KeyCode.I;

    CharacterController m_CharacterController;

    
    
    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
    }
    void Update()
    {
#if UNITY_EDITOR
        FreezMouseDebug();
#endif
        HandleAim();
        HandleMovement();

        if (Moving)
        {
            //m_PitchCtrlAnimation.Play();
        }
            //m_PitchCtrlAnimation.Stop();
    }

    private void FreezMouseDebug()
    {
        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
            m_AngleLocked = !m_AngleLocked;
        if (Input.GetKeyDown(m_DebugLockKeyCode))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }
    }

    #region "Aim Code"
    private void HandleAim()
    {
        float l_HorizontalMovement = Input.GetAxis("Mouse X");
        float l_VerticalMovement = Input.GetAxis("Mouse Y");

        if (m_AngleLocked)
        {
            l_HorizontalMovement = 0;
            l_VerticalMovement = 0;
        }

        CamRotating = l_HorizontalMovement != 0 || l_VerticalMovement != 0;

        float l_YawInverted = m_YawInverted ? -1.0f : 1.0f;
        float l_PitchInverted = m_PitchInverted ? -1 : 1;

    

        m_Yaw += m_YawSpeed * l_HorizontalMovement * Time.deltaTime * l_YawInverted;
        m_Pitch += m_PitchSpeed * l_VerticalMovement * Time.deltaTime * l_PitchInverted;

        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);

        transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
        m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);
    }
    #endregion

    #region "Shoot Code"
    #endregion

    #region "Movement Code"
    private void HandleMovement()
    {
        Vector3 l_Movement = Vector3.zero;

        HandleWalk(ref l_Movement);
        HandleJump(ref l_Movement);

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        HandleCollisions(l_CollisionFlags);
    }

    private void HandleCollisions(CollisionFlags collisionFlags)
    {
        if ((collisionFlags & CollisionFlags.CollidedBelow) != 0)
        {
            m_VerticalSpeed = 0.0f;
            Jumping = false;
        }
        if ((collisionFlags & CollisionFlags.CollidedAbove) != 0 && m_VerticalSpeed > 0.0f)
            m_VerticalSpeed = 0.0f;

    }

    private void HandleJump(ref Vector3 movement)
    {
        if (CanJump())
        {
            m_VerticalSpeed = m_JumpSpeed;
            Jumping = true;
        }

        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;
        movement.y = m_VerticalSpeed * Time.deltaTime;
    }
    private bool CanJump()
    {
        if (m_VerticalSpeed == 0) m_LastTimeOnGround = Time.time;

        bool l_InCoyoteTime = Time.time - m_LastTimeOnGround <= m_CoyoteTime;
        return Input.GetKeyDown(m_JumpKeyCode) && l_InCoyoteTime;
    }
    private void HandleWalk(ref Vector3 movement)
    {
        float l_Speed = m_WalkingSpeed;

        if (CanSprint())
            l_Speed = m_SprintSpeed;


        float l_YawInRadians = m_Yaw * Mathf.Deg2Rad;
        float l_Yaw90InRadians = (m_Yaw + 90.0f) * Mathf.Deg2Rad;

        Vector3 l_Forward = new(Mathf.Sin(l_YawInRadians), 0.0f, Mathf.Cos(l_YawInRadians));
        Vector3 l_Right = new(Mathf.Sin(l_Yaw90InRadians), 0.0f, Mathf.Cos(l_Yaw90InRadians));

        if (Input.GetKey(m_LeftKeyCode))
            movement -= l_Right;
        if (Input.GetKey(m_RightKeyCode))
            movement += l_Right;

        if (Input.GetKey(m_DownKeyCode))
            movement -= l_Forward;
        if (Input.GetKey(m_UpKeyCode))
            movement += l_Forward;

        Moving = movement != Vector3.zero;
        Sprinting = l_Speed == m_SprintSpeed && Moving;
        movement.Normalize();
        movement *= l_Speed * Time.deltaTime;
    }
    private bool CanSprint()
    {
        return Input.GetKey(m_SprintKeyCode);
    }

    #endregion

    #region "ICollector"

    #endregion
    public void AddTorque(Vector2 torque, float time)
    {
        StartCoroutine(AddTorqueOverTime(torque, time));
    }

    private IEnumerator AddTorqueOverTime(Vector2 torque, float time)
    {
        float l_timer = time;

        while (l_timer > 0)
        {
            m_Pitch += torque.y * Time.deltaTime;
            m_Yaw += torque.x * Time.deltaTime;
            l_timer -= Time.deltaTime;
            yield return null;
        }
    }

    void DeactivatePlayer()
    {
        this.enabled = false;
    }

    public void RestartElement()
    {
        this.enabled = true;
    }
}

using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEditor.UIElements;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    public bool CamRotating { get; private set; }
    public bool Moving { get; private set; }
    public bool Sprinting { get; private set; }
    public bool Jumping { get; private set; }

    Rigidbody m_Rigidbody;

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
    public PortalGun m_PortalGun;

    [Header("Movement")]
    public LayerMask m_GroundLayer;
    [SerializeField] float m_WalkingSpeed;
    [SerializeField] float m_SprintSpeed;
    [SerializeField] float m_CrouchingSpeed;
    [SerializeField] float m_JumpForce;
    [SerializeField] float m_CoyoteTime;
    float m_LastTimeOnGround;
    float m_VerticalSpeed;
    public Vector3 MovementDirection => m_MovementDirection;
    Vector3 m_MovementDirection;

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
    public Camera Camera => m_Camera;

    [Header("Animations")]
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
        m_Rigidbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void FixedUpdate()
    {
#if UNITY_EDITOR
        FreezMouseDebug();
#endif
        HandleAim();
        HandleMovement();
        HandleJump();

        if (Moving)
        {
            //m_PitchCtrlAnimation.Play();
        }
        //else
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
    public void SetYaw(float _Yaw)
    {
        m_Yaw = _Yaw;
    }

    #endregion

    #region "Shoot Code"
    #endregion

    #region "Movement Code"
    private void HandleMovement()
    {
        Vector3 l_HorizontalDirection = GetHorizontalDirection();
        float l_Speed = m_WalkingSpeed;
        if (CanSprint())
            l_Speed = m_SprintSpeed;
        Vector3 l_Velocity = l_Speed * l_HorizontalDirection;
        m_Rigidbody.velocity = new Vector3(l_Velocity.x, m_Rigidbody.velocity.y, l_Velocity.z);
    }
    private Vector3 GetHorizontalDirection()
    {
        float l_YawInRadians = m_Yaw * Mathf.Deg2Rad;
        float l_Yaw90InRadians = (m_Yaw + 90.0f) * Mathf.Deg2Rad;

        Vector3 l_Forward = new(Mathf.Sin(l_YawInRadians), 0.0f, Mathf.Cos(l_YawInRadians));
        Vector3 l_Right = new(Mathf.Sin(l_Yaw90InRadians), 0.0f, Mathf.Cos(l_Yaw90InRadians));

        Vector3 l_Direction = Vector3.zero;

        if (Input.GetKey(m_LeftKeyCode))
            l_Direction -= l_Right;
        if (Input.GetKey(m_RightKeyCode))
            l_Direction += l_Right;

        if (Input.GetKey(m_DownKeyCode))
            l_Direction -= l_Forward;
        if (Input.GetKey(m_UpKeyCode))
            l_Direction += l_Forward;

        l_Direction.y = 0;
        l_Direction.Normalize();
        return l_Direction;
    }

    private void HandleJump()
    {
        if (CanJump())
        {
            Vector3 l_Force = Vector3.up * m_JumpForce;
            m_Rigidbody.AddForce(l_Force);
        }
    }
    private bool CanJump()
    {
        if (OnGround()) m_LastTimeOnGround = Time.time;

        bool l_InCoyoteTime = Time.time - m_LastTimeOnGround <= m_CoyoteTime;
        return Input.GetKeyDown(m_JumpKeyCode) && l_InCoyoteTime;
    }

    private bool OnGround()
    {
        Ray l_Ray = new Ray(transform.position, Vector3.down);
        float l_Distance = 0.001f;
        if (Physics.Raycast(l_Ray, l_Distance, m_GroundLayer.value))
        {
            var temp = m_Rigidbody.velocity;
            temp.y = 0;
            m_Rigidbody.velocity = temp;
            return true;
        }
        return false;
        
    }

    private bool CanSprint()
    {
        return Input.GetKey(m_SprintKeyCode);
    }

    #endregion

    #region "ICollector"

    #endregion
    void DeactivatePlayer()
    {
        this.enabled = false;
    }

    public void RestartElement()
    {
        this.enabled = true;
    }
}

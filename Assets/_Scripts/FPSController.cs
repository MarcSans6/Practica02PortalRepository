using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEditor.UIElements;
using UnityEngine;

[RequireComponent(typeof(PortablePlayer))]
[RequireComponent(typeof(Rigidbody))]
public class FPSController : MonoBehaviour
{
    public bool CamRotating { get; private set; }
    public bool Moving { get; private set; }
    public bool Sprinting { get; private set; }
    public bool Jumping { get; private set; }


    [Header("References")]
    [SerializeField] Transform m_YawController;
    public Transform YawController => m_YawController;
    [SerializeField] Transform m_PitchController;
    public Transform PitchController => m_PitchController;
    [SerializeField] Transform m_RollController;
    public Transform RollController => m_RollController;
    Rigidbody m_Rigidbody;
    [SerializeField] Camera m_Camera;
    public Camera Camera => m_Camera;
    PortablePlayer m_PortablePlayer;

    [Header("Aim")]
    [SerializeField] float m_YawSpeed;
    [SerializeField] float m_PitchSpeed;
    [SerializeField] float m_RollRecoverySpeed;
    [Space]
    [SerializeField] float m_MaxPitch;
    [SerializeField] float m_MinPitch;
    [Space]
    [SerializeField] bool m_YawInverted;
    [SerializeField] bool m_PitchInverted;
    float m_Yaw;
    float m_Pitch;
    float m_Roll;

    [Header("Movement")]
    public LayerMask m_GroundLayer;
    [SerializeField] float m_WalkingSpeed;
    [SerializeField] float m_SprintSpeed;
    [Range(.0f, 1f)]
    [SerializeField] float m_HorizontalDrag;
    [SerializeField] float m_JumpForce;
    [SerializeField] float m_CoyoteTime;
    float m_LastTimeOnGround;
    Vector3 m_LastHorizontalDirection;
    CapsuleCollider m_Collider;
    bool m_IsMidAirAfterWarp;

    [Header("Input")]
    [SerializeField] KeyCode m_UpKeyCode = KeyCode.W;
    [SerializeField] KeyCode m_LeftKeyCode = KeyCode.A;
    [SerializeField] KeyCode m_DownKeyCode = KeyCode.S;
    [SerializeField] KeyCode m_RightKeyCode = KeyCode.D;
    [SerializeField] KeyCode m_JumpKeyCode = KeyCode.Space;
    [SerializeField] KeyCode m_SprintKeyCode = KeyCode.LeftShift;
    [Space]

    [Header("Animations")]
    [SerializeField] Animation m_PitchCtrlAnimation;
    [SerializeField] AnimationClip m_CameraShakeAnim;
    [SerializeField] Animator m_PlayerCameraAnimator;
 
    [Header("Debug")]
    [SerializeField] bool m_AngleLocked;
    [Space]
    [SerializeField] KeyCode m_DebugLockAngleKeyCode = KeyCode.O;
    [SerializeField] KeyCode m_DebugLockKeyCode = KeyCode.I;

    private void OnEnable() => m_PortablePlayer.OnWarp += OnWarp;
    private void OnDisable() => m_PortablePlayer.OnWarp -= OnWarp;

    void OnWarp() => m_IsMidAirAfterWarp = true;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<CapsuleCollider>();
        m_PortablePlayer = GetComponent<PortablePlayer>();
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
    }

    private void Update()
    {
        HandleJump();
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
        m_Roll = Mathf.MoveTowards(m_Roll, 0.0f, m_RollRecoverySpeed * Time.deltaTime);

        SetYaw(m_Yaw);
        SetPitch(m_Pitch);
        SetRoll(m_Roll);
    }
    public void SetYaw(float _Yaw)
    {
        m_Yaw = _Yaw;
        m_YawController.rotation = Quaternion.Euler(0.0f, 
            m_Yaw, 0.0f);
    }

    public void SetPitch(float _Pitch)
    {
        m_Pitch = _Pitch;
        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);
        m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);
    }

    public void SetRoll(float _Roll)
    {
        m_Roll = _Roll;
        m_RollController.localRotation = Quaternion.Euler(0.0f, 0.0f, m_Roll);
    }


    #endregion

    #region "Shoot Code"
    #endregion

    #region "Movement Code"
    private void HandleMovement()
    {
        Vector3 l_HorizontalInput = GetHorizontalInput();

        // If l_HorizontalInput is different from last frame, then the rb velocity is set to 0.
        if (ChangedDirection(l_HorizontalInput)) 
        {
            ResetHorizontalVelocity();
        }
        ///

        // We initialize the force we would apply this frame based on the speed we want to achieve.
        float l_Force = GetHorizontalForceFromSpeed(m_WalkingSpeed);
        if (CanSprint())
            l_Force = GetHorizontalForceFromSpeed(m_SprintSpeed);
        /// 

        // We just add force this frame in 2 different situations:
        //  - if the player is grounded
        //  - if the player is mid air but makes an input.
        //  - on both situations, the player can't be midair after a warp.
        // This way, if there is no input in air, the player follows the inertia.
        if (OnGround() || l_HorizontalInput != Vector3.zero && !m_IsMidAirAfterWarp )
        {
            m_Rigidbody.AddForce(l_Force * l_HorizontalInput);
        }
        ///

        // We just apply drag if the player input is 0. This way if the player is moving the drag isn't applied.
        if (l_HorizontalInput == Vector3.zero && !m_IsMidAirAfterWarp)
            ApplyDrag(m_HorizontalDrag);
        ///    
    }

    //Resets the horizontal velocity to 0.
    private void ResetHorizontalVelocity()
    {
        Vector3 l_Vel = m_Rigidbody.velocity;
        l_Vel.x = 0;
        l_Vel.z = 0;
        m_Rigidbody.velocity = l_Vel;
    }

    // Returns true if the player was moving in a direction last frame and if it changed this frame.
    private bool ChangedDirection(Vector3 l_HorizontalDirection)
    {
        bool l_Changed = m_LastHorizontalDirection != Vector3.zero && 
            m_LastHorizontalDirection != l_HorizontalDirection && l_HorizontalDirection != Vector3.zero;
        m_LastHorizontalDirection = l_HorizontalDirection;
        return l_Changed;
    }

    // Simulates the drag effect of the rigidbody, just in the horizontal axis.
    private void ApplyDrag(float l_HorizontalDrag)
    {
        Vector3 l_Vel = m_Rigidbody.velocity;
        l_Vel.x *= 1.0f - l_HorizontalDrag;
        l_Vel.z *= 1.0f - l_HorizontalDrag;
        m_Rigidbody.velocity = l_Vel;
    }

    // Returns a force that applied makes the rb horizontal velocity magnitude the speed we desire.
    private float GetHorizontalForceFromSpeed(float _DesiredSpeed)
    {
        float l_HorizontalSpeed = new Vector3(m_Rigidbody.velocity.x, .0f, m_Rigidbody.velocity.z).magnitude;
        float l_TrueDesiredSpeed = l_HorizontalSpeed > _DesiredSpeed ? 0 : _DesiredSpeed - l_HorizontalSpeed;
        float l_Force = m_Rigidbody.mass * (l_TrueDesiredSpeed / Time.fixedDeltaTime);
        return l_Force;
    }
    private Vector3 GetHorizontalInput()
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
            Jump();
        }
    }

    private void Jump()
    {
        Vector3 l_Temp = m_Rigidbody.velocity;
        l_Temp.y = 0;
        m_Rigidbody.velocity = l_Temp;
        Vector3 l_Force = Vector3.up * m_JumpForce;
        m_Rigidbody.AddForce(l_Force);
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
        float l_Distance = (m_Collider.height / 2) + 0.01f;
        if (Physics.Raycast(l_Ray, l_Distance, m_GroundLayer.value))
        {
            m_IsMidAirAfterWarp = false;
            return true;
        }
        return false;

    }

    private bool CanSprint()
    {
        return Input.GetKey(m_SprintKeyCode);
    }

    #endregion

    public void RestartElement()
    {
        this.enabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(m_PitchController.position, m_PitchController.position + m_PitchController.forward * 2);
    }
}

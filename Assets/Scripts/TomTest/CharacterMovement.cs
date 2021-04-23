using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour, IUpdateUser
{
    #region CustomUpdate
    [SerializeField]
    private SO_UpdateLayerSettings m_UpdateSettings = null;
    private void OnEnable()
    {
        m_UpdateSettings.Bind(this);
    }
    private void OnDisable()
    {
        m_UpdateSettings.Unbind(this);
    }
    #endregion
    #region Variables
    #region Scripts
    private CharacterController m_CharacterController = null;
    private CharacterInfos m_CharacterInfos;
    private PlayerInfos m_PlayerInfos;
    #endregion
    #region Movement
    private Vector3 m_PlayerGeneralDirection = Vector3.zero;
    private Vector3 m_PlayerDesiredDirection = Vector3.zero;
    #endregion
    #region Speed
    private float m_MaxCharacterSpeed = 10f;
    private float m_CharacterSpeed = 10f;
    private float m_EditableCharacterSpeed = 1f;
    #endregion
    #region Grounded
    [SerializeField]
    private Transform m_PlayerGroundCheck = null;
    [SerializeField]
    private float m_GroundDistance = 0.5f;
    [SerializeField]
    private LayerMask m_GroundMask = 0;
    private bool m_IsGrounded = false;
    #endregion
    #region Jump
    private AnimationCurve m_GroundJumpCurve = null;
    private AnimationCurve m_AirJumpCurve = null;
    
    private Vector3 m_JumpMark = Vector3.zero; 
    
    private bool m_IsGroundJumping = false;
    private bool m_IsAirJumping = false;

    private float m_TimerGroundJump = 0f;
    private float m_TimerAirJump = 0f;

    private float m_CharacterMaxAirJump = 1;
    private float m_CharacterAirJump = 0;
    #endregion
    #region Gravity
    private float m_CharacterMaxGravity = -10f;
    private float m_CharacterGravity = -10f;
    #endregion
    #region Orientation
    private float m_CharacterOrientation = 1;
    #endregion
    #region MovementMomentum
    private AnimationCurve m_CharacterStartVelocity = null;
    private AnimationCurve m_CharacterEndVelocity = null;

    private float m_StartVelocityTimer = 0;
    private float m_EndVelocityTimer = 0;

    private bool m_StartVelocityCheck = false;
    private bool m_EndVelocityCheck = false;

    private Vector3 m_PastDirection = Vector3.zero;
    #endregion
    #endregion

    #region Awake/Start
    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_CharacterInfos = GetComponent<CharacterInfos>();
        m_PlayerInfos = GetComponent<PlayerInfos>();
    }

    private void Start()
    {
        m_MaxCharacterSpeed = m_CharacterInfos.MaxCharacterSpeed;
        m_GroundJumpCurve = m_CharacterInfos.GroundJumpCurve;
        m_AirJumpCurve = m_CharacterInfos.AirJumpCurve;
        m_CharacterMaxAirJump = m_CharacterInfos.CharacterMaxAirJump;
        m_CharacterMaxGravity = m_CharacterInfos.CharacterMaxGravity;
        m_CharacterStartVelocity = m_CharacterInfos.CharacterStartVelocity;
        m_CharacterEndVelocity = m_CharacterInfos.CharacterEndVelocity;
}
    #endregion

    #region Update
    public void CustomUpdate(float p_DeltaTime)
    {
        m_CharacterOrientation = transform.localScale.x;
        m_IsGrounded = Physics.CheckBox(m_PlayerGroundCheck.position, new Vector3(m_GroundDistance, 0.3f, 0.3f), Quaternion.identity, m_GroundMask);
        if (m_IsGrounded)
        {
            m_CharacterAirJump = m_CharacterMaxAirJump;
            m_CharacterGravity = m_CharacterMaxGravity;
        }
        if (m_IsGroundJumping)
        {
            m_TimerGroundJump += p_DeltaTime;
        }
        else if (m_IsAirJumping)
        {
            m_TimerAirJump += p_DeltaTime;
        }
        else
        {
            m_PlayerGeneralDirection += new Vector3(0, m_CharacterGravity * p_DeltaTime, 0);
        }
        if (Physics.Raycast(transform.position, transform.up, 1f))
        {
            m_IsGroundJumping = false;
            m_IsAirJumping = false;
        }
        GroundJump();
        AirJump();
        StartGainVelocity(p_DeltaTime);
        EndLossVelocity(p_DeltaTime);
        m_PlayerGeneralDirection += m_PlayerDesiredDirection * m_CharacterSpeed * p_DeltaTime * m_EditableCharacterSpeed;
        m_CharacterController.Move(m_PlayerGeneralDirection);
        m_PlayerGeneralDirection = Vector3.zero;
    }
    #endregion

    #region Functions
    #region Movement
    public void PlayerHorizontalMovement(InputAction.CallbackContext p_Context)
    {
        if (p_Context.control.device.deviceId == m_PlayerInfos.DeviceID)
        {
            if (p_Context.started)
            {
                m_StartVelocityCheck = true;
                m_EndVelocityCheck = false;
                m_StartVelocityTimer = 0;
                m_EndVelocityTimer = 0;
            }
            if (p_Context.performed)
            {
                if (p_Context.ReadValue<Vector2>().x >= 0.2f && p_Context.ReadValue<Vector2>().y < 0.9f && p_Context.ReadValue<Vector2>().y > -0.7f || p_Context.ReadValue<Vector2>().x <= -0.2f && p_Context.ReadValue<Vector2>().y < 0.9f && p_Context.ReadValue<Vector2>().y > -0.7f)
                {
                    m_PlayerDesiredDirection = new Vector3(p_Context.ReadValue<Vector2>().x, 0, 0);
                    m_GroundJumpCurve.keys[m_GroundJumpCurve.keys.Length - 1].time = 0.41f;
                    m_AirJumpCurve.keys[m_AirJumpCurve.keys.Length - 1].time = 0.41f;
                    m_PastDirection = m_PlayerDesiredDirection;
                }
                if (p_Context.ReadValue<Vector2>().y >= 0.9f || p_Context.ReadValue<Vector2>().y <= -0.7f)
                {
                    m_PlayerDesiredDirection = Vector3.zero;
                    m_GroundJumpCurve.keys[m_GroundJumpCurve.keys.Length - 1].time = 0.41f + 0.6f * Mathf.Abs(p_Context.ReadValue<Vector2>().x);
                    m_AirJumpCurve.keys[m_AirJumpCurve.keys.Length - 1].time = 0.41f + 0.6f * Mathf.Abs(p_Context.ReadValue<Vector2>().x);
                }
            }
            if (p_Context.canceled)
            {
                m_StartVelocityCheck = false;
                m_EndVelocityCheck = true;
                m_StartVelocityTimer = 0;
                m_EndVelocityTimer = 0;
                m_PlayerDesiredDirection = Vector3.zero;
            }
        }
    }

    private void StartGainVelocity(float p_DeltaTime)
    {
        if (m_StartVelocityCheck)
        {
            m_StartVelocityTimer += p_DeltaTime;
            m_CharacterSpeed = m_MaxCharacterSpeed * m_CharacterStartVelocity.Evaluate(m_StartVelocityTimer);
        }
        if (m_StartVelocityTimer >= m_CharacterStartVelocity.keys[m_CharacterStartVelocity.keys.Length - 1].time)
        {
            m_StartVelocityCheck = false;
            m_StartVelocityTimer = 0;
        }
    }

    private void EndLossVelocity(float p_DeltaTime)
    {
        if (m_EndVelocityCheck)
        {
            m_EndVelocityTimer += p_DeltaTime;
            m_CharacterSpeed = m_MaxCharacterSpeed * m_CharacterEndVelocity.Evaluate(m_EndVelocityTimer) * Mathf.Abs(m_PastDirection.x);
            m_PlayerDesiredDirection = m_PastDirection;
        }
        if (m_EndVelocityTimer >= m_CharacterEndVelocity.keys[m_CharacterEndVelocity.keys.Length - 1].time)
        {
            m_EndVelocityCheck = false;
            m_EndVelocityTimer = 0;
        }
    }

    public void PlayerAirDownMovement(InputAction.CallbackContext p_Context)
    {
        if (p_Context.control.device.deviceId == m_PlayerInfos.DeviceID)
        {
            if (p_Context.performed)
            {
                if (!m_IsAirJumping && !m_IsGroundJumping)
                {
                    if (p_Context.ReadValue<Vector2>().y < -0.8f)
                    {
                        m_CharacterGravity *= 1.1f;
                    }
                }
            }
        }
    }
    #endregion
    #region Orientation
    public void PlayerMovementOrientation(InputAction.CallbackContext p_Context)
    {
        if (p_Context.control.device.deviceId == m_PlayerInfos.DeviceID)
        {
            if (p_Context.performed)
            {
                if (p_Context.ReadValue<Vector2>().x >= 0.2f && p_Context.ReadValue<Vector2>().x != 0 || p_Context.ReadValue<Vector2>().x <= -0.2f && p_Context.ReadValue<Vector2>().x != 0)
                {
                    PlayerOrientation(p_Context.ReadValue<Vector2>().x / Mathf.Abs(p_Context.ReadValue<Vector2>().x));
                }
            }
        }
    }

    public void PlayerOrientation(float p_Orientation)
    {
        transform.localScale = new Vector3(p_Orientation / Mathf.Abs(p_Orientation), transform.localScale.y, transform.localScale.z);
    }
    #endregion
    #region Jump
    public void Jump(InputAction.CallbackContext p_Context)
    {
        if (p_Context.control.device.deviceId == m_PlayerInfos.DeviceID)
        {
            if (m_IsGrounded)
            {
                if (p_Context.started)
                {
                    m_JumpMark = m_PlayerGroundCheck.position;
                    m_IsGroundJumping = true;
                    m_TimerGroundJump = 0;
                }
            }
            else
            {
                if (m_CharacterAirJump > 0)
                {
                    if (p_Context.started)
                    {
                        m_CharacterAirJump -= 1;
                        m_JumpMark = m_PlayerGroundCheck.position;
                        m_IsAirJumping = true;
                        m_TimerAirJump = 0;
                        m_IsGroundJumping = false;
                    }
                }
            }
        }
    }

    private void GroundJump()
    {

        if (m_IsGroundJumping)
        {
            if (m_TimerGroundJump <= m_GroundJumpCurve.keys[m_GroundJumpCurve.keys.Length - 1].time)
            {
                m_CharacterController.enabled = false;
                m_CharacterController.transform.position = new Vector3(transform.position.x, m_JumpMark.y + m_GroundJumpCurve.Evaluate(m_TimerGroundJump) + (transform.position.y - m_PlayerGroundCheck.position.y), transform.position.z);
                m_CharacterController.enabled = true;
            }
            else
            {
                m_TimerGroundJump = 0;
                m_IsGroundJumping = false;
            }
        }
    }

    private void AirJump()
    {
        if (m_IsAirJumping)
        {
            if (m_TimerAirJump <= m_AirJumpCurve.keys[m_AirJumpCurve.keys.Length - 1].time)
            {
                m_CharacterController.enabled = false;
                m_CharacterController.transform.position = new Vector3(transform.position.x, m_JumpMark.y + m_AirJumpCurve.Evaluate(m_TimerAirJump) + (transform.position.y - m_PlayerGroundCheck.position.y), transform.position.z);
                m_CharacterController.enabled = true;
            }
            else
            {
                m_TimerAirJump = 0;
                m_IsAirJumping = false;
            }
        }
    }
    #endregion
    #endregion

    #region Assessor
    public bool IsGrounded
    {
        get { return m_IsGrounded; }
    }

    public float CharacterOrientation
    {
        get { return m_CharacterOrientation; }
    }

    public float EditableCharacterSpeed
    {
        get { return m_EditableCharacterSpeed; }

        set { m_EditableCharacterSpeed = value; }
    }
    #endregion
}

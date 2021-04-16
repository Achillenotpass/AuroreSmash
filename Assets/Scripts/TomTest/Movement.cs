using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    private CharacterController m_CharacterController = null;

    private Vector3 m_PlayerGeneralDirection = Vector3.zero;

    private Vector3 m_PlayerDesiredDirection = Vector3.zero;

    [SerializeField]
    private float m_MaxCharacterSpeed = 10f;
    private float m_CharacterSpeed = 10f;
    
    [SerializeField]
    private AnimationCurve m_GroundJumpCurve = null;
    [SerializeField]
    private AnimationCurve m_AirJumpCurve = null;

    [SerializeField]
    private Transform m_PlayerGroundCheck = null;
    [SerializeField]
    private float m_GroundDistance = 0.5f;
    [SerializeField]
    private LayerMask m_GroundMask = 0;
    private bool m_IsGrounded = false;

    [SerializeField]
    private float m_CharacterMaxGravity = -10f;
    private float m_CharacterGravity = -10f;

    private Vector3 m_JumpMark = Vector3.zero;

    private bool m_IsGroundJumping = false;
    private bool m_IsAirJumping = false;

    private float m_TimerGroundJump = 0f;
    private float m_TimerAirJump = 0f;

    [SerializeField]
    private float m_CharacterMaxAirJump = 1;
    private float m_CharacterAirJump = 0;

    [SerializeField]
    private AnimationCurve m_CharacterStartVelocity = null;
    [SerializeField]
    private AnimationCurve m_CharacterEndVelocity = null;

    private float m_StartVelocityTimer = 0;
    private float m_EndVelocityTimer = 0;

    private bool m_StartVelocityCheck = false;
    private bool m_EndVelocityCheck = false;

    private float m_CharacterOrientation = 1;

    private float m_CharacterMovementControl = 1;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        m_CharacterOrientation = transform.localScale.x;
        m_IsGrounded = Physics.CheckBox(m_PlayerGroundCheck.position, new Vector3(m_GroundDistance, 0.3f, 0.3f), Quaternion.identity, m_GroundMask);
        if(m_IsGrounded)
        {
            m_CharacterAirJump = m_CharacterMaxAirJump;
            m_CharacterGravity = m_CharacterMaxGravity;
        }
        if (m_IsGroundJumping)
        {
            m_TimerGroundJump += Time.deltaTime;
        }
        else if (m_IsAirJumping)
        {
            m_TimerAirJump += Time.deltaTime;
        }
        else
        {
            m_PlayerGeneralDirection += new Vector3(0, m_CharacterGravity * Time.deltaTime, 0);
        }
        GroundJump();
        AirJump();
        m_PlayerGeneralDirection += m_PlayerDesiredDirection * m_CharacterSpeed * Time.deltaTime;
        m_PlayerGeneralDirection *= m_CharacterMovementControl;
        m_CharacterController.Move(m_PlayerGeneralDirection);
        m_PlayerGeneralDirection = Vector3.zero;
    }

    public void PlayerHorizontalMovement(InputAction.CallbackContext p_Context)
    {
        if (p_Context.started)
        {
            m_StartVelocityCheck = true;
        }
        if(p_Context.performed)
        {
            m_PlayerDesiredDirection = new Vector3(p_Context.ReadValue<Vector2>().x, 0, 0);
            m_GroundJumpCurve.keys[m_GroundJumpCurve.keys.Length - 1].time = 0.41f + 0.6f * Mathf.Abs(p_Context.ReadValue<Vector2>().x);
            m_AirJumpCurve.keys[m_AirJumpCurve.keys.Length - 1].time = 0.41f + 0.6f * Mathf.Abs(p_Context.ReadValue<Vector2>().x);
        }
        if(p_Context.canceled)
        {
            m_StartVelocityCheck = false;
            m_EndVelocityCheck = false;
            m_PlayerDesiredDirection = Vector3.zero;
        }
    }

    public void PlayerOrientation(InputAction.CallbackContext p_Context)
    {
        if(p_Context.performed)
        {
            if(p_Context.ReadValue<Vector2>().x != 0)
                transform.localScale = new Vector3(p_Context.ReadValue<Vector2>().x/ Mathf.Abs(p_Context.ReadValue<Vector2>().x), transform.localScale.y, transform.localScale.z);
        }
    }

    public void PlayerAirDownMovement(InputAction.CallbackContext p_Context)
    {
        if(p_Context.performed)
        {
            if(!m_IsAirJumping && ! m_IsGroundJumping)
            {
                if (p_Context.ReadValue<Vector2>().y < -0.8f)
                {
                    Debug.Log("aaa");
                    m_CharacterGravity *= 1.1f;
                }
            }
        }
    }

    public void Jump(InputAction.CallbackContext p_Context)
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
            if(m_CharacterAirJump > 0)
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

    private void GroundJump()
    {
        if(Physics.Raycast(transform.position, transform.up, 1f, LayerMask.NameToLayer("Ground")) && m_IsGroundJumping)
        {
            m_IsGroundJumping = false;
        }
        if (m_IsGroundJumping)
        {
            //Debug.Log(m_JumpMark.y + m_GroundJumpCurve.keys[m_GroundJumpCurve.keys.Length - 1].value + (transform.position.y - m_PlayerGroundCheck.position.y));
            if (m_TimerGroundJump <= m_GroundJumpCurve.keys[m_GroundJumpCurve.keys.Length - 1].time)
                /*transform.position.y < m_JumpMark.y + m_JumpCurve.keys[m_JumpCurve.keys.Length - 1].value + (transform.position.y - m_PlayerGroundCheck.position.y) - 0.01f)*/
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
        if (Physics.Raycast(transform.position, transform.up, 1f, LayerMask.NameToLayer("Ground")) && m_IsAirJumping)
        {
            m_IsAirJumping = false;
        }
        if (m_IsAirJumping)
        {
            //Debug.Log(m_JumpMark.y + m_AirJumpCurve.keys[m_AirJumpCurve.keys.Length - 1].value + (transform.position.y - m_PlayerGroundCheck.position.y));
            if (m_TimerAirJump <= m_AirJumpCurve.keys[m_AirJumpCurve.keys.Length - 1].time)
            /*transform.position.y < m_JumpMark.y + m_JumpCurve.keys[m_JumpCurve.keys.Length - 1].value + (transform.position.y - m_PlayerGroundCheck.position.y) - 0.01f)*/
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

    public bool IsGrounded
    {
        get { return m_IsGrounded; }
    }

    public float CharacterOrientation
    {
        get { return m_CharacterOrientation; }
    }

    public float CharacterSpeed
    {
        get { return m_CharacterSpeed; }

        set { m_CharacterSpeed = value; }
    }

}
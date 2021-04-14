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
    private float m_Speed = 10f;
    
    [SerializeField]
    private AnimationCurve m_JumpCurve = null;

    [SerializeField]
    private Transform m_PlayerGroundCheck = null;
    [SerializeField]
    private float m_GroundDistance = 0.5f;
    [SerializeField]
    private LayerMask m_GroundMask = 0;
    private bool m_IsGrounded = false;

    private Transform m_JumpMark = null;

    private bool m_IsGroundJumping = false;

    [SerializeField]
    private float m_TimerGroundJump = 0.25f;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        m_IsGrounded = Physics.CheckBox(m_PlayerGroundCheck.position, new Vector3(m_GroundDistance, 0.2f, 0.2f), Quaternion.identity, m_GroundMask);
        //Physics.CheckSphere(m_PlayerGroundCheck.position, m_GroundDistance, m_GroundMask);
        GroundJump();
        if (!m_IsGroundJumping)
            m_PlayerGeneralDirection += new Vector3(0, -10 * Time.deltaTime, 0);
        m_PlayerGeneralDirection += m_PlayerDesiredDirection * m_Speed * Time.deltaTime;
        m_CharacterController.Move(m_PlayerGeneralDirection);
        m_PlayerGeneralDirection = Vector3.zero;
    }

    public void PlayerHorizontalMovement(InputAction.CallbackContext p_Context)
    {
        if(p_Context.started)
        {
        }
        if(p_Context.performed)
        {
            m_PlayerDesiredDirection = new Vector3(p_Context.ReadValue<Vector2>().x, 0, 0).normalized;
        }
        if(p_Context.canceled)
        {
            m_PlayerDesiredDirection = Vector3.zero;
        }
    }

    public void Jump(InputAction.CallbackContext p_Context)
    {
        if (m_IsGrounded)
        {
            Debug.Log("aaa");
            if (p_Context.started)
            {
                Debug.Log("bbb");
                m_JumpMark = m_PlayerGroundCheck;
                m_IsGroundJumping = true;
            }
        }
        else
        {
        }
    }

    public void GroundJump()
    {
        if(m_IsGroundJumping)
        {
            Debug.Log("ccc");
            m_TimerGroundJump += Time.deltaTime;
            transform.position = m_JumpMark.position + new Vector3(0, m_JumpCurve.Evaluate(m_TimerGroundJump), 0);
        }
    }
}
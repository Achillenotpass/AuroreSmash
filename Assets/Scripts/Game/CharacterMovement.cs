using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;


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
    private Vector3 m_PlayerExternalDirection = Vector3.zero;
    private Vector3 m_PlayerEjectionDirection = Vector3.zero;

    private Vector3 m_JoystickValue = Vector3.zero;
    private float m_BaseZ = 0.0f;
    public float BaseZ { get { return m_BaseZ; } set { m_BaseZ = value; } }
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
    [SerializeField]
    private GameObject m_CharacterView = null;
    private Vector2 m_InputOrientation = Vector2.one;
    #endregion
    #region MovementMomentum
    private AnimationCurve m_CharacterStartVelocity = null;
    private AnimationCurve m_CharacterEndGroundVelocity = null;
    private AnimationCurve m_CharacterEndAirVelocity = null;

    private float m_StartVelocityTimer = 0;
    private float m_EndGroundVelocityTimer = 0;
    private float m_EndAirVelocityTimer = 0;

    private bool m_StartVelocityCheck = false;
    private bool m_EndGroundVelocityCheck = false;
    private bool m_EndGroundVelocityInverseCheck = false;
    private bool m_EndAirVelocityCheck = false;
    private bool m_EndAirVelocityInverseCheck = false;

    private Vector3 m_PastDirection = Vector3.zero;
    #endregion
    #region Events
    [SerializeField]
    private MovementEvents m_MovementEvents = new MovementEvents();
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
        m_CharacterEndGroundVelocity = m_CharacterInfos.CharacterEndGroundVelocity;
        m_CharacterEndAirVelocity = m_CharacterInfos.CharacterEndAirVelocity;
        m_CharacterSpeed = m_MaxCharacterSpeed;
}
    #endregion

    #region Update
    public void CustomUpdate(float p_DeltaTime)
    {
        m_CharacterOrientation = m_CharacterView.transform.localScale.x;
        if(!m_IsGrounded && Physics.CheckBox(m_PlayerGroundCheck.position, new Vector3(m_GroundDistance, 0.3f, 0.3f), Quaternion.identity, m_GroundMask))
        {
            m_MovementEvents.m_Landing.Invoke();
            if(m_CharacterInfos.CurrentCharacterState != CharacterState.Moving)
                m_CharacterInfos.CurrentCharacterState = CharacterState.Idle;
        }
        m_IsGrounded = Physics.CheckBox(m_PlayerGroundCheck.position, new Vector3(m_GroundDistance, 0.3f, 0.3f), Quaternion.identity, m_GroundMask);
        


        if (m_IsGrounded)
        {
            m_MovementEvents.m_OnGround.Invoke();
            m_CharacterAirJump = m_CharacterMaxAirJump;
            m_CharacterGravity = m_CharacterMaxGravity;
            if(m_CharacterInfos.CurrentCharacterState == CharacterState.Moving)
            {
                ChangeOrientationGround();
                m_MovementEvents.m_EventGroundMovement.Invoke();
            }
            if (m_EndAirVelocityCheck)
            {
                m_EndAirVelocityInverseCheck = true;
            }
        }
        else
        {
            m_MovementEvents.m_NotOnGround.Invoke();
        }
        if (m_EndGroundVelocityCheck && m_CharacterSpeed <= 0 || m_EndAirVelocityCheck && m_CharacterSpeed <= 0)
        {
            m_EndGroundVelocityInverseCheck = true;
            m_EndAirVelocityInverseCheck = true;
        }
        if (m_IsGroundJumping)
        {
            m_TimerGroundJump += p_DeltaTime;
        }
        else if (m_IsAirJumping)
        {
            m_TimerAirJump += p_DeltaTime;
        }
        else if(m_CharacterInfos.CurrentCharacterState != CharacterState.Hitlag)
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
        EndGroundLossVelocity(p_DeltaTime);
        EndAirLossVelocity(p_DeltaTime);
        StartGainVelocity(p_DeltaTime);
        m_PlayerGeneralDirection += m_PlayerExternalDirection * p_DeltaTime;
        m_PlayerGeneralDirection += m_PlayerEjectionDirection * p_DeltaTime;
        m_PlayerGeneralDirection += m_PlayerDesiredDirection * m_CharacterSpeed * p_DeltaTime * m_EditableCharacterSpeed;
        m_CharacterController.Move(m_PlayerGeneralDirection);
        m_PlayerGeneralDirection = Vector3.zero;
        CheckAnimation();

        m_CharacterController.Move(new Vector3(transform.position.x, transform.position.y, m_BaseZ) - transform.position);
    }
    #endregion

    #region Functions
    #region Movement
    public void PlayerHorizontalMovement(InputAction.CallbackContext p_Context)
    {
        if (m_CharacterInfos.CurrentCharacterState == CharacterState.Moving 
            || m_CharacterInfos.CurrentCharacterState == CharacterState.Idle 
            || m_CharacterInfos.CurrentCharacterState == CharacterState.Attacking && !m_IsGrounded)
        {
            if (p_Context.started)
            {
                if (m_IsGrounded)
                {
                    m_StartVelocityCheck = true;
                    m_MovementEvents.m_EventStartGroundMovement.Invoke();
                }
                else
                    m_CharacterSpeed = m_MaxCharacterSpeed;
                m_EndGroundVelocityCheck = false;
                m_EndAirVelocityCheck = false;
                m_StartVelocityTimer = 0;
                m_EndGroundVelocityTimer = 0;
                m_EndAirVelocityTimer = 0;
            }
            if (p_Context.performed)
            {
                m_JoystickValue = p_Context.ReadValue<Vector2>();
                if (m_JoystickValue.x >= 0.2f && m_JoystickValue.y < 0.9f && m_JoystickValue.y > -0.7f || m_JoystickValue.x <= -0.2f && m_JoystickValue.y < 0.9f && m_JoystickValue.y > -0.7f)
                {
                    m_PlayerDesiredDirection = new Vector3(m_JoystickValue.x/Mathf.Abs(m_JoystickValue.x), 0, 0);
                    m_GroundJumpCurve.keys[m_GroundJumpCurve.keys.Length - 1].time = 0.41f;
                    m_AirJumpCurve.keys[m_AirJumpCurve.keys.Length - 1].time = 0.41f;
                    m_PastDirection = m_PlayerDesiredDirection;
                    if (m_CharacterInfos.CurrentCharacterState != CharacterState.Attacking)
                    {
                        m_CharacterInfos.CurrentCharacterState = CharacterState.Moving;
                    }
                    m_MovementEvents.m_EventMovement.Invoke();

                }
                if (m_JoystickValue.y >= 0.9f || m_JoystickValue.y <= -0.7f)
                {
                    m_PlayerDesiredDirection = Vector3.zero;
                    m_GroundJumpCurve.keys[m_GroundJumpCurve.keys.Length - 1].time = 0.41f + 0.6f * Mathf.Abs(m_JoystickValue.x);
                    m_AirJumpCurve.keys[m_AirJumpCurve.keys.Length - 1].time = 0.41f + 0.6f * Mathf.Abs(m_JoystickValue.x);
                }
            }
        }
        if (p_Context.canceled)
        {
            m_StartVelocityCheck = false;
            if (m_IsGrounded)
                m_EndGroundVelocityCheck = true;
            else
            {
                m_EndAirVelocityCheck = true;
            }
            m_StartVelocityTimer = 0;
            m_EndGroundVelocityTimer = 0;
            m_EndAirVelocityTimer = 0;
            m_PlayerDesiredDirection = Vector3.zero;
            m_MovementEvents.m_EventEndMovement.Invoke();
        }
    }

    private void StartGainVelocity(float p_DeltaTime)
    {
        if (m_CharacterStartVelocity != null)
        {
            if (m_StartVelocityTimer >= m_CharacterStartVelocity.keys[m_CharacterStartVelocity.keys.Length - 1].time)
            {
                m_StartVelocityCheck = false;
                m_StartVelocityTimer = 0;
            }
        }
        else
        {
            Debug.Log("m_CharacterStartVelocity is null");
        }
        if (m_StartVelocityCheck)
        {
            m_CharacterInfos.CurrentCharacterState = CharacterState.Moving;
            m_StartVelocityTimer += p_DeltaTime;
            m_CharacterSpeed = m_MaxCharacterSpeed * m_CharacterStartVelocity.Evaluate(m_StartVelocityTimer);
        }
    }

    private void EndGroundLossVelocity(float p_DeltaTime)
    {
        if (m_CharacterInfos.CurrentCharacterState == CharacterState.Moving)
        {
            if (m_CharacterSpeed <= 0)
            {
                ZeroCharacterSpeed();
                m_EndGroundVelocityCheck = false;
                m_EndGroundVelocityTimer = 0;
            }
        }
        if (m_EndGroundVelocityInverseCheck)
        {
            m_EndGroundVelocityCheck = false;
            m_EndGroundVelocityInverseCheck = false;
            ZeroCharacterSpeed();
            m_EndGroundVelocityTimer = 0;
        }
        if (m_CharacterInfos.CurrentCharacterState == CharacterState.Moving)
        {
            if (m_EndGroundVelocityCheck)
            {
                m_StartVelocityCheck = false;
                m_EndGroundVelocityTimer += p_DeltaTime;
                m_CharacterSpeed = m_MaxCharacterSpeed * m_CharacterEndGroundVelocity.Evaluate(m_EndGroundVelocityTimer) * Mathf.Abs(m_PastDirection.x);
                m_PlayerDesiredDirection = m_PastDirection;
            }
        }
    }

    private void EndAirLossVelocity(float p_DeltaTime)
    {
        if (m_CharacterSpeed <= 0)
        {
            ZeroCharacterSpeed();
            m_EndAirVelocityCheck = false;
            m_EndAirVelocityTimer = 0;
        }
        if (m_EndAirVelocityInverseCheck)
        {
            m_EndAirVelocityCheck = false;
            m_EndAirVelocityInverseCheck = false;
            ZeroCharacterSpeed();
            m_EndAirVelocityTimer = 0;
        }
        if (m_EndAirVelocityCheck)
        {
            m_StartVelocityCheck = false;
            m_EndAirVelocityTimer += p_DeltaTime;
            m_CharacterSpeed = m_MaxCharacterSpeed * m_CharacterEndAirVelocity.Evaluate(m_EndAirVelocityTimer) * Mathf.Abs(m_PastDirection.x);
            m_PlayerDesiredDirection = m_PastDirection;
        }
    }

    private void ZeroCharacterSpeed()
    {
        m_CharacterSpeed = 0;
        m_PlayerDesiredDirection = Vector3.zero;
        if(m_CharacterInfos.CurrentCharacterState == CharacterState.Moving)
            m_CharacterInfos.CurrentCharacterState = CharacterState.Idle;
    }

    public void PlayerAirDownMovement(InputAction.CallbackContext p_Context)
    {
        if (m_CharacterInfos.CurrentCharacterState == CharacterState.Moving || m_CharacterInfos.CurrentCharacterState == CharacterState.Idle)
        {
            if (p_Context.performed && !m_IsAirJumping && !m_IsGroundJumping && p_Context.ReadValue<Vector2>().y < -0.8f)
            {
                m_MovementEvents.m_EventJumpDownMovement.Invoke();
                m_CharacterGravity *= 1.1f;
            }
        }
    }

    public void ZeroMovementInput()
    {
        m_PlayerDesiredDirection = Vector3.zero;
        if (m_EndGroundVelocityCheck)
            m_EndGroundVelocityInverseCheck = true;
        if (m_EndAirVelocityCheck)
            m_EndGroundVelocityInverseCheck = true;
    }

    public void TerminateMomentum()
    {
        m_CharacterSpeed = 0;
        m_PlayerDesiredDirection = Vector3.zero;
        m_EndGroundVelocityCheck = false;
        m_EndGroundVelocityTimer = 0;
    }
    #endregion
    #region Orientation
    public void PlayerMovementOrientation(InputAction.CallbackContext p_Context)
    {
        if (m_CharacterInfos.CurrentCharacterState == CharacterState.Moving || m_CharacterInfos.CurrentCharacterState == CharacterState.Idle)
        {
            if (p_Context.performed
                && p_Context.ReadValue<Vector2>().x >= 0.2f && p_Context.ReadValue<Vector2>().x != 0 || p_Context.ReadValue<Vector2>().x <= -0.2f && p_Context.ReadValue<Vector2>().x != 0)
            {
                //PlayerOrientation(p_Context.ReadValue<Vector2>().x);
                m_InputOrientation = p_Context.ReadValue<Vector2>();
            }
        }
    }

    private void ChangeOrientationGround()
    {
        PlayerOrientation(m_InputOrientation.x);
    }

    public void PlayerOrientation(float p_Orientation)
    {
        if (m_IsGrounded)
        {
            if (m_CharacterView.transform.localScale.x / Mathf.Abs(m_CharacterView.transform.localScale.x) != p_Orientation / Mathf.Abs(p_Orientation))
            {
                m_MovementEvents.m_EventChangeOrientation.Invoke();
                m_CharacterView.transform.localScale = new Vector3(-1 * m_CharacterView.transform.localScale.x, m_CharacterView.transform.localScale.y, m_CharacterView.transform.localScale.z);
            }
        }
    }
    #endregion
    #region Jump
    public void Jump(InputAction.CallbackContext p_Context)
    {
        if (m_CharacterInfos.CurrentCharacterState == CharacterState.Moving || m_CharacterInfos.CurrentCharacterState == CharacterState.Idle)
        {
            m_CharacterInfos.CurrentCharacterState = CharacterState.Moving;
            m_EndAirVelocityCheck = false;
            m_EndGroundVelocityCheck = false;
            if (m_IsGrounded)
            {
                if (p_Context.started)
                {
                    m_MovementEvents.m_EventBeginGroundJump.Invoke();
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
                        m_MovementEvents.m_EventBeginAirJump.Invoke();
                        m_CharacterAirJump -= 1;
                        m_JumpMark = m_PlayerGroundCheck.position;
                        m_IsAirJumping = true;
                        m_TimerAirJump = 0;
                        m_IsGroundJumping = false;
                        /*for (int i = 0; i < FindObjectOfType<FeedbackCaller>().FeedbackList.FeedbackArray.Length; i++)
                        {
                            Debug.Log("bbb");
                            if (FindObjectOfType<FeedbackCaller>().FeedbackList.FeedbackArray[i].name == "DoubleJump")
                            {
                                Debug.Log("ccc");
                                FindObjectOfType<FeedbackCaller>().FeedbackList.FeedbackArray[i].InstantiateVFXAnimation(null, transform.position + new Vector3(0, -0.5f, 0), i);
                            }
                        }*/
                    }
                }
            }
        }
    }

    private void GroundJump()
    {
        if (m_IsGroundJumping)
        {
            if (m_TimerGroundJump <= m_GroundJumpCurve.keys[m_GroundJumpCurve.keys.Length - 1].time
                && ( m_CharacterInfos.CurrentCharacterState == CharacterState.Moving || m_CharacterInfos.CurrentCharacterState == CharacterState.Idle ))
            {
                m_MovementEvents.m_EventJump.Invoke();
                m_CharacterController.enabled = false;
                m_CharacterController.transform.position = new Vector3(transform.position.x, m_JumpMark.y + m_GroundJumpCurve.Evaluate(m_TimerGroundJump) + (transform.position.y - m_PlayerGroundCheck.position.y), transform.position.z);
                m_CharacterController.enabled = true;
                m_CharacterInfos.CurrentCharacterState = CharacterState.Moving;
            }
            else
            {
                if (m_CharacterInfos.CurrentCharacterState == CharacterState.Moving || m_CharacterInfos.CurrentCharacterState == CharacterState.Idle)
                {
                    m_MovementEvents.m_EventEndJump.Invoke();
                }
                m_TimerGroundJump = 0;
                m_IsGroundJumping = false;
            }
        }
    }

    private void AirJump()
    {
        if (m_IsAirJumping)
        {
            if (m_TimerAirJump <= m_AirJumpCurve.keys[m_AirJumpCurve.keys.Length - 1].time
                && ( m_CharacterInfos.CurrentCharacterState == CharacterState.Moving || m_CharacterInfos.CurrentCharacterState == CharacterState.Idle ))
            {
                m_MovementEvents.m_EventJump.Invoke();
                m_CharacterController.enabled = false;
                m_CharacterController.transform.position = new Vector3(transform.position.x, m_JumpMark.y + m_AirJumpCurve.Evaluate(m_TimerAirJump) + (transform.position.y - m_PlayerGroundCheck.position.y), transform.position.z);
                m_CharacterController.enabled = true;
                m_CharacterInfos.CurrentCharacterState = CharacterState.Moving;
            }
            else
            {
                if (m_CharacterInfos.CurrentCharacterState == CharacterState.Moving || m_CharacterInfos.CurrentCharacterState == CharacterState.Idle)
                {
                    m_MovementEvents.m_EventEndJump.Invoke();
                }
                m_TimerAirJump = 0;
                m_IsAirJumping = false;
            }
        }
    }
    #endregion
    private void CheckAnimation()
    {
        if (m_IsGroundJumping && m_CharacterInfos.CurrentCharacterState == CharacterState.Moving)
        {
            //Ground jump
            m_MovementEvents.m_EventBeginGroundJump.Invoke();
        }
        else if (m_IsAirJumping && m_CharacterInfos.CurrentCharacterState == CharacterState.Moving)
        {
            //Air jump
            m_MovementEvents.m_EventBeginAirJump.Invoke();
        }
        else if (m_IsGrounded)
        {
            if (m_CharacterInfos.CurrentCharacterState == CharacterState.Idle)
            {
                //Idle
                m_MovementEvents.m_StartGroundIdleAnimation.Invoke();
            }
            else if (m_CharacterInfos.CurrentCharacterState == CharacterState.Moving)
            {
                //Move
                m_MovementEvents.m_StartMoveAnimation.Invoke();
            }
        }
        else
        {
            if (m_CharacterInfos.CurrentCharacterState == CharacterState.Moving || m_CharacterInfos.CurrentCharacterState == CharacterState.Idle)
            {
                //Idle air
                m_MovementEvents.m_StartAirIdleAnimation.Invoke();
            }
        }
    }
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

    public Vector3 PlayerDesiredDirection
    {
        get { return m_PlayerDesiredDirection; }

        set { m_PlayerDesiredDirection = value; }
    }

    public float EditableCharacterSpeed
    {
        get { return m_EditableCharacterSpeed; }

        set { m_EditableCharacterSpeed = value; }
    }

    public Vector3 PlayerExternalDirection
    {
        get { return m_PlayerExternalDirection; }
        set { m_PlayerExternalDirection = value; }
    }

    public Vector3 PlayerEjectionDirection
    {
        get { return m_PlayerEjectionDirection; }
        set { m_PlayerEjectionDirection = value; }
    }

    public bool EndGroundVelocityInverseCheck
    {
        get { return m_EndGroundVelocityInverseCheck; }

        set { m_EndGroundVelocityInverseCheck = value; }
    }

    public bool EndAirVelocityInverseCheck
    {
        get { return m_EndAirVelocityInverseCheck; }

        set { m_EndAirVelocityInverseCheck = value; }
    }
    #endregion
}

#region Events
[System.Serializable]
public class MovementEvents
{
    [SerializeField]
    public UnityEvent m_OnGround;
    [SerializeField]
    public UnityEvent m_NotOnGround;

    [SerializeField]
    public UnityEvent m_EventStartGroundMovement;
    [SerializeField]
    public UnityEvent m_EventStartAirMovement;
    [SerializeField]
    public UnityEvent m_EventMovement;
    [SerializeField]
    public UnityEvent m_EventGroundMovement;
    [SerializeField]
    public UnityEvent m_EventEndMovement;

    [SerializeField]
    public UnityEvent m_EventBeginGroundJump;
    [SerializeField]
    public UnityEvent m_EventBeginAirJump;
    [SerializeField]
    public UnityEvent m_EventJump;
    [SerializeField]
    public UnityEvent m_EventEndJump;
    [SerializeField]
    public UnityEvent m_EventJumpDownMovement;
    [SerializeField]
    public UnityEvent m_Landing;

    [SerializeField]
    public UnityEvent m_EventChangeOrientation;

    public UnityEvent m_StartMoveAnimation;
    public UnityEvent m_StartGroundIdleAnimation;
    public UnityEvent m_StartAirIdleAnimation;
}
#endregion
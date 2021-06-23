using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class CharacterEjection : MonoBehaviour, IUpdateUser
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
    [SerializeField]
    private LayerMask m_ReboundLayerMask = 0;
    private float m_EjectionAngle = 0;
    private float m_EjectionPower = 0;
    [SerializeField]
    private float m_MinTimerEjection = 1;
    [SerializeField]
    private float m_MultiplicatorAt0HP = 3.0f;
    private float m_TimerEjection = 1;

    private bool m_IsEjected = false;

    private Health m_Health = null;

    private CharacterMovement m_CharacterMovement = null;

    private CharacterController m_CharacterController = null;

    private CharacterInfos m_CharacterInfos = null;

    private Vector3 m_BasePosition = Vector3.zero;

    private Vector3 m_ActualEjectionPoint = Vector3.zero;
    private Vector3 m_PreviousEjectionPoint = Vector3.zero;

    private float m_EjectionDirection = 0.0f;

    [SerializeField]
    private EjectionEvents m_EjectionEvents = new EjectionEvents();
    #endregion

    #region Awake/Start
    private void Awake()
    {
        m_CharacterMovement = GetComponent<CharacterMovement>();
        m_CharacterInfos = GetComponent<CharacterInfos>();
        m_CharacterController = GetComponent<CharacterController>();
        m_Health = GetComponent<Health>();
    }

    private void Start()
    {
    }
    #endregion

    #region Update
    public void CustomUpdate(float p_DeltaTime)
    {
        EjectionTime();
        CalculateEjection();
    }
    #endregion


    public void Ejection(float p_EjectionPower, float p_EjectionAngle, Vector3 p_AttackerPosition, float p_HitLagTime)
    {
        if (!m_IsEjected)
        {
            m_CharacterInfos.CurrentCharacterState = CharacterState.Hitlag;
            m_EjectionPower = p_EjectionPower;
            m_EjectionAngle = p_EjectionAngle;
            m_BasePosition = transform.position;
            m_IsEjected = true;
            m_TimerEjection = m_MinTimerEjection + p_HitLagTime * (m_MultiplicatorAt0HP * ((m_Health.MaxHealth - m_Health.CurrentHealth) / m_Health.MaxHealth));

            if (p_AttackerPosition.x > transform.position.x)
            {
                m_EjectionDirection = -1.0f;
            }
            else
            {
                m_EjectionDirection = 1.0f;
            }
            if(p_EjectionPower + 1f + ((100f - (m_Health.CurrentHealth / m_Health.MaxHealth * 100f)) / 45f) >= 15)
            {
                m_EjectionEvents.m_TakePowerfullHit.Invoke();
                m_EjectionEvents.m_EjectionPowerfull.Invoke();
            }
            else
            {
                m_EjectionEvents.m_TakeHit.Invoke();
                m_EjectionEvents.m_Ejection.Invoke();
            }
        }
    }

    private void CalculateEjection()
    {
        if (m_IsEjected)
        {
            m_CharacterInfos.CurrentCharacterState = CharacterState.Hitlag;
            m_ActualEjectionPoint = new Vector3(m_EjectionPower * (m_EjectionDirection * (1f + ((100f - (m_Health.CurrentHealth / m_Health.MaxHealth * 100f)) / 45f)) * Mathf.Cos(m_EjectionAngle * Mathf.Deg2Rad) * m_TimerEjection), m_EjectionPower * (1f + ((100f - (m_Health.CurrentHealth / m_Health.MaxHealth * 100f)) / 45f)) * Mathf.Sin(m_EjectionAngle * Mathf.Deg2Rad) * m_TimerEjection, 0);
            if (m_PreviousEjectionPoint != Vector3.zero)
                m_CharacterMovement.PlayerEjectionDirection += m_ActualEjectionPoint - m_PreviousEjectionPoint;
            else
                m_CharacterMovement.PlayerEjectionDirection = m_ActualEjectionPoint;
            m_PreviousEjectionPoint = m_ActualEjectionPoint;
            m_CharacterMovement.EndGroundVelocityInverseCheck = true;
            m_CharacterMovement.EndAirVelocityInverseCheck = true;
            m_CharacterMovement.EditableCharacterSpeed = 0;

            if (Physics.Raycast(transform.position, m_CharacterMovement.PlayerEjectionDirection, out RaycastHit l_HitInfo, 0.5f, m_ReboundLayerMask))
            {
                Vector3 m_NewDirection = Vector3.zero;
                float l_Angle = 0.0f;

                l_Angle = Vector3.SignedAngle(-m_CharacterMovement.PlayerEjectionDirection, l_HitInfo.normal, Vector3.forward);
                m_NewDirection = Vector3.RotateTowards(m_CharacterMovement.PlayerEjectionDirection, l_HitInfo.normal, l_Angle * 6.28f / 360.0f, 0.0f);

                float l_NewAngle = Vector3.SignedAngle(Vector3.right, m_NewDirection, Vector3.forward);
                m_EjectionAngle = l_NewAngle;
            }
        }
    }
    public void EjectionTime()
    {
        if(m_IsEjected)
        {
            if (m_TimerEjection <= 0)
            {
                m_CharacterInfos.CurrentCharacterState = CharacterState.Idle;
                m_CharacterMovement.PlayerEjectionDirection = Vector3.zero;
                m_IsEjected = false;
                m_CharacterMovement.EditableCharacterSpeed = 1;
                m_CharacterMovement.ZeroMovementInput();
            }
            m_TimerEjection -= Time.deltaTime;
        }
    }

    public void InterruptEjection()
    {
        m_TimerEjection = 0.0f;
    }
}

[System.Serializable]
public class EjectionEvents
{
    [SerializeField]
    public UnityEvent m_TakeHit;

    [SerializeField]
    public UnityEvent m_TakePowerfullHit;

    [SerializeField]
    public UnityEvent m_Ejection;

    [SerializeField]
    public UnityEvent m_EjectionPowerfull;
} 
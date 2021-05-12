using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private int m_TestAngle = 0;
    [SerializeField]
    private int m_TestPower = 0;
    [SerializeField]
    private float m_MaxTimerEjection = 1;
    private float m_TimerEjection = 1;

    private bool m_IsEjected = false;

    private Health m_Health = null;

    private CharacterMovement m_CharacterMovement = null;

    private CharacterInfos m_CharacterInfos = null;

    private Vector3 m_BasePosition = Vector3.zero;

    private Vector3 m_ActualEjectionPoint = Vector3.zero;
    private Vector3 m_PreviousEjectionPoint = Vector3.zero;
    #endregion

    #region Awake/Start
    private void Awake()
    {
        m_CharacterMovement = GetComponent<CharacterMovement>();
        m_CharacterInfos = GetComponent<CharacterInfos>();
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
        Debug.Log(m_CharacterInfos.CurrentCharacterState);
        CalculateEjection();
    }
    #endregion


    public void Ejection(InputAction.CallbackContext p_Context/*float p_EjectionPower, float p_EjectionAngle*/)
    {
        m_BasePosition = transform.position;
        m_IsEjected = true;
        m_TimerEjection = 1;
    }

    private void CalculateEjection()
    {
        if (m_IsEjected)
        {
            m_CharacterInfos.CurrentCharacterState = CharacterState.Hitlag;
            m_ActualEjectionPoint = new Vector3(m_TestPower /** (m_Health.CurrentHealth / m_Health.MaxHealth * 100)*/ * Mathf.Cos(m_TestAngle * Mathf.Deg2Rad) * m_TimerEjection, m_TestPower * Mathf.Sin(m_TestAngle * Mathf.Deg2Rad) * m_TimerEjection, 0);
            if (m_PreviousEjectionPoint != Vector3.zero)
                m_CharacterMovement.PlayerEjectionDirection += m_ActualEjectionPoint - m_PreviousEjectionPoint;
            else
                m_CharacterMovement.PlayerEjectionDirection = m_ActualEjectionPoint;
            m_PreviousEjectionPoint = m_ActualEjectionPoint;
            m_CharacterMovement.EndGroundVelocityInverseCheck = true;
            m_CharacterMovement.EndAirVelocityInverseCheck = true;
            m_CharacterMovement.EditableCharacterSpeed = 0;
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
            }
            m_TimerEjection -= Time.deltaTime;
        }
    }
}
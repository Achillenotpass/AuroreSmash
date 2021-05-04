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
    private float m_TimerEjection = 1;

    private bool m_IsEjected = false;

    private Health m_Health = null;

    private CharacterMovement m_CharacterMovement = null;
    #endregion

    #region Awake/Start
    private void Awake()
    {
        m_CharacterMovement = GetComponent<CharacterMovement>();
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
    }
    #endregion


    public void CalculateEjection(InputAction.CallbackContext p_Context/*float p_EjectionPower, float p_EjectionAngle*/)
    {
        m_CharacterMovement.PlayerEjectionDirection = new Vector3(Mathf.Cos(m_TestAngle * Mathf.Deg2Rad), Mathf.Sin(m_TestAngle * Mathf.Deg2Rad), 0) * m_TestPower;
        m_IsEjected = true;
        m_TimerEjection = 0.1f * m_TestPower * (100.1f - (100 * (m_Health.CurrentHealth / m_Health.MaxHealth)));
        Debug.Log(m_TimerEjection);
    }

    public void EjectionTime()
    {
        if(m_IsEjected)
        {
            if (m_TimerEjection <= 0)
            {
                m_CharacterMovement.PlayerEjectionDirection = Vector3.zero;
                m_IsEjected = false;
            }
            m_TimerEjection -= Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawRay(transform.position, new Vector3(Mathf.Cos(m_TestAngle * Mathf.Deg2Rad), Mathf.Sin(m_TestAngle * Mathf.Deg2Rad), 0) * 100f);
    }
}
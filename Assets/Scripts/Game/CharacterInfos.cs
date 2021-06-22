using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class CharacterInfos : MonoBehaviour
{
    #region EventsVariables
    [SerializeField]
    private InfosEvents m_InfosEvents = new InfosEvents();
    #endregion

    [SerializeField]
    private SO_Character m_Character = null;
    public SO_Character Character { get { return m_Character; } set { m_Character = value; } }
    private CharacterState m_CurrentCharacterState = CharacterState.Idle;
    public CharacterState CurrentCharacterState 
    { 
        get { return m_CurrentCharacterState; } 

        set 
        { 
            if(m_CurrentCharacterState == CharacterState.Moving && value != CharacterState.Moving)
            {
                m_InfosEvents.m_EventChangeStateFromMoving.Invoke();
            }
            if(m_CurrentCharacterState == CharacterState.Hitlag && value != CharacterState.Hitlag)
            {
                m_InfosEvents.m_EventChangeEndEjectionState.Invoke();
            }
            m_CurrentCharacterState = value; 
            if(m_CurrentCharacterState == CharacterState.Grabbed)
            {
                m_InfosEvents.m_EventChangeStateToGrabbed.Invoke();
            }
        } 
    }

    [SerializeField]
    private float m_MaxCharacterSpeed = 10f;
    public float MaxCharacterSpeed { get { return m_MaxCharacterSpeed; } }

    [SerializeField]
    private AnimationCurve m_GroundJumpCurve = null;
    public AnimationCurve GroundJumpCurve { get { return m_GroundJumpCurve; } }
    [SerializeField]
    private AnimationCurve m_AirJumpCurve = null;
    public AnimationCurve AirJumpCurve { get { return m_AirJumpCurve; } }

    [SerializeField]
    private float m_CharacterMaxAirJump = 1;
    public float CharacterMaxAirJump { get { return m_CharacterMaxAirJump; } }

    [SerializeField]
    private float m_CharacterMaxGravity = -10f;
    public float CharacterMaxGravity { get { return m_CharacterMaxGravity; } }

    [SerializeField]
    private AnimationCurve m_CharacterStartVelocity = null;
    public AnimationCurve CharacterStartVelocity { get { return m_CharacterStartVelocity; } }
    [SerializeField]
    private AnimationCurve m_CharacterEndGroundVelocity = null;
    public AnimationCurve CharacterEndGroundVelocity { get { return m_CharacterEndGroundVelocity; } }
    [SerializeField]
    private AnimationCurve m_CharacterEndAirVelocity = null;
    public AnimationCurve CharacterEndAirVelocity { get { return m_CharacterEndAirVelocity; } }

    private bool m_CanMove = true;
    [SerializeField]
    public bool CanMove
    {
        get { return m_CanMove; }
        set { m_CanMove = value; }
    }
}

#region Events
[System.Serializable]
public class InfosEvents
{
    [SerializeField]
    public UnityEvent m_EventChangeStateFromMoving;

    [SerializeField]
    public UnityEvent m_EventChangeStateToGrabbed;

    [SerializeField]
    public UnityEvent m_EventChangeEndEjectionState;
}
#endregion 

public enum CharacterState
{
    Idle,
    Moving,
    Jumping,
    Attacking,
    Shielding,
    Grabbing,
    Grabbed,
    Hitlag,
    Lag,
    Death,
}
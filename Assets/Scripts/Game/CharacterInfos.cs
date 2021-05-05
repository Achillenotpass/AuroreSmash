using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfos : MonoBehaviour
{
    private CharacterState m_CurrentCharacterState = CharacterState.Idle;
    public CharacterState CurrentCharacterState { get { return m_CurrentCharacterState; } set { m_CurrentCharacterState = value; } }

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

public enum CharacterState
{
    Idle,
    Moving,
    Attacking,
    Shielding,
    Grabbing,
    Grabbed,
    Hitlag,
    Lag,
    Death,
}
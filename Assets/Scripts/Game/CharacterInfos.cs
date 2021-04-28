using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfos : MonoBehaviour
{
    private bool m_IsAttacking = false;
    public bool IsAttacking { get { return m_IsAttacking; } set { m_IsAttacking = value; } }

    private bool m_IsHitLagging = false;
    public bool IsHitLagging { get { return m_IsHitLagging; } set { m_IsHitLagging = value; } }

    private bool m_IsShielding = false;
    public bool IsShielding { get { return m_IsShielding; } set { m_IsShielding = value; } }

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
    private AnimationCurve m_CharacterEndVelocity = null;
    public AnimationCurve CharacterEndVelocity { get { return m_CharacterEndVelocity; } }
}

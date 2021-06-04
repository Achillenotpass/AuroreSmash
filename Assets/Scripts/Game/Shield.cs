using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Shield : MonoBehaviour, IUpdateUser
{

    #region UpdateMangaer
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
    private Vector2 m_CurrentShieldDirection = Vector2.zero;
    private ShieldState m_CurrentShieldState = ShieldState.NoShield;
    private ShieldState m_NewShieldState = ShieldState.NoShield;
    [SerializeField]
    private float m_DamageReduction = 80.0f;

    private int m_CurrentFrameCount = 0;
    [SerializeField]
    private int m_LagBefore = 1;
    [SerializeField]
    private int m_LagAfter = 1;
    [SerializeField]
    private int m_LagAfterOmni = 1;
    [Header("Functionment")]
    [SerializeField]
    private List<GameObject> m_ShieldPositions = new List<GameObject>();
    private PlayerInfos m_PlayerInfos = null;
    private CharacterInfos m_CharacterInfos = null;

    [SerializeField]
    private UnityEvent m_StartShieldEvent = null;
    #endregion


    private void Awake()
    {
        m_PlayerInfos = GetComponent<PlayerInfos>();
        m_CharacterInfos = GetComponent<CharacterInfos>();
    }
    public void CustomUpdate(float p_DeltaTime)
    {
        if (m_CharacterInfos.CurrentCharacterState == CharacterState.Shielding)
        {
            m_StartShieldEvent.Invoke();
            switch (m_CurrentShieldState)
            {
                case ShieldState.LagBefore:
                    m_CurrentFrameCount = m_CurrentFrameCount + 1;
                    if (m_CurrentFrameCount > m_LagBefore)
                    {
                        m_NewShieldState = GetShieldState(m_CurrentShieldDirection);
                        m_CurrentFrameCount = 0;
                    }
                    break;
                case ShieldState.LagAfter:
                    m_CurrentFrameCount = m_CurrentFrameCount + 1;
                    if (m_CurrentFrameCount > m_LagAfter)
                    {
                        m_NewShieldState = ShieldState.NoShield;
                        m_CharacterInfos.CurrentCharacterState = CharacterState.Idle;
                        m_CurrentFrameCount = 0;
                    }
                    break;
                case ShieldState.LagAfterOmni:
                    m_CurrentFrameCount = m_CurrentFrameCount + 1;
                    if (m_CurrentFrameCount > m_LagAfterOmni)
                    {
                        m_NewShieldState = ShieldState.NoShield;
                        m_CharacterInfos.CurrentCharacterState = CharacterState.Idle;
                        m_CurrentFrameCount = 0;
                    }
                    break;
                default:
                    m_NewShieldState = GetShieldState(m_CurrentShieldDirection);
                    break;
            }
        }
        else
        {
            m_NewShieldState = ShieldState.NoShield;
        }
        if (m_CurrentShieldState != m_NewShieldState)
        {
            m_CurrentShieldState = m_NewShieldState;
            SetShieldRotation(m_CurrentShieldState);
        }
    }
    public void ShieldInput(InputAction.CallbackContext p_Context)
    {
        if (p_Context.ReadValueAsButton())
        {
            if (m_CharacterInfos.CurrentCharacterState == CharacterState.Idle || m_CharacterInfos.CurrentCharacterState == CharacterState.Moving)
            {
                m_CharacterInfos.CurrentCharacterState = CharacterState.Shielding;
                m_CurrentShieldState = ShieldState.LagBefore;
                m_NewShieldState = ShieldState.LagBefore;
            }
        }
        else
        {
            if (m_CharacterInfos.CurrentCharacterState == CharacterState.Shielding)
            {
                switch (m_CurrentShieldState)
                {
                    case ShieldState.Omni:
                        m_NewShieldState = ShieldState.LagAfterOmni;
                        m_CurrentShieldState = ShieldState.LagAfterOmni;
                        SetShieldRotation(ShieldState.NoShield);
                        m_CurrentFrameCount = 0;
                        break;
                    default:
                        m_NewShieldState = ShieldState.LagAfter;
                        m_CurrentShieldState = ShieldState.LagAfter;
                        SetShieldRotation(ShieldState.NoShield);
                        m_CurrentFrameCount = 0;
                        break;
                }
            }
        }
    }
    public void ShieldDirectionInput(InputAction.CallbackContext p_Context)
    {
        if (p_Context.ReadValue<Vector2>().magnitude > 0.2f)
        {
            m_CurrentShieldDirection = p_Context.ReadValue<Vector2>();
        }
        else
        {
            m_CurrentShieldDirection = Vector2.zero;
        }
    }
    private ShieldState GetShieldState(Vector2 p_ShieldInputDirection)
    {
        if (p_ShieldInputDirection.magnitude > 0.2f)
        {
            float l_Angle = Vector2.Angle(Vector2.right, p_ShieldInputDirection.normalized);
            float l_Increment = 180.0f / 5.0f;
            if (p_ShieldInputDirection.y < 0 && l_Angle > l_Increment && l_Angle < 180.0f - l_Increment)
            {
                return ShieldState.Omni;
            }
            else if (l_Angle <= l_Increment)
            {
                return ShieldState.Right;
            }
            else if (l_Angle <= 2 * l_Increment)
            {
                return ShieldState.UpRight;
            }
            else if (l_Angle <= 3 * l_Increment)
            {
                return ShieldState.Up;
            }
            else if (l_Angle <= 4 * l_Increment)
            {
                return ShieldState.UpLeft;
            }
            else
            {
                return ShieldState.Left;
            }
        }
        return ShieldState.Right;
    }
    private void SetShieldRotation(ShieldState p_ShieldState)
    {
        switch (p_ShieldState)
        {
            case ShieldState.NoShield:
                ActivateShieldPart(-1);
                break;
            case ShieldState.Right:
                ActivateShieldPart(1);
                break;
            case ShieldState.UpRight:
                ActivateShieldPart(2);
                break;
            case ShieldState.Up:
                ActivateShieldPart(3);
                break;
            case ShieldState.UpLeft:
                ActivateShieldPart(4);
                break;
            case ShieldState.Left:
                ActivateShieldPart(5);
                break;
            case ShieldState.Omni:
                ActivateShieldPart(0);
                break;
        }
    }
    private void ActivateShieldPart(int p_ShieldPart)
    {
        foreach (GameObject l_ShieldPart in m_ShieldPositions)
        {
            l_ShieldPart.SetActive(false);
        }
        if (p_ShieldPart != -1)
        {
            m_ShieldPositions[p_ShieldPart].SetActive(true);
        }
    }

    public void TakeShieldDamages(SO_HitBox p_Hitbox, Vector3 p_AttackerPosition)
    {
        SO_HitBox l_ShieldedHitBox = new SO_HitBox();
        l_ShieldedHitBox.Damages = p_Hitbox.Damages * m_DamageReduction / 100.0f;
        l_ShieldedHitBox.EjectionPower = 0.0f;
        GetComponent<Health>().TakeDamages(l_ShieldedHitBox, p_AttackerPosition);
    }
    public void TakeShieldDamages(SO_Projectile p_Projectile, Vector3 p_ProjectileObjectPosition)
    {
        SO_Projectile l_ShieldedProjectile = new SO_Projectile();
        l_ShieldedProjectile.Damages = p_Projectile.Damages * m_DamageReduction / 100.0f;
        l_ShieldedProjectile.EjectionPower = 0.0f;
        GetComponent<Health>().TakeDamages(l_ShieldedProjectile, p_ProjectileObjectPosition);
    }

    public enum ShieldState
    {
        NoShield,
        LagBefore,
        LagAfter,
        LagAfterOmni,
        Right,
        UpRight,
        Up,
        UpLeft,
        Left,
        Omni,
    }
}



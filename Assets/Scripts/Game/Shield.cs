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

    private bool m_IsShielding = false;
    private Vector2 m_CurrentShieldDirection = Vector2.zero;
    private ShieldState m_CurrentShieldState = ShieldState.NoShield;
    private ShieldState m_NewShieldState = ShieldState.NoShield;
    [SerializeField]
    private float m_DamageReduction = 80.0f;
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


    private void Awake()
    {
        m_PlayerInfos = GetComponent<PlayerInfos>();
        m_CharacterInfos = GetComponent<CharacterInfos>();
    }
    public void CustomUpdate(float p_DeltaTime)
    {
        if (m_CharacterInfos.IsShielding)
        {
            m_NewShieldState = GetShieldState(m_CurrentShieldDirection);
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
        if (p_Context.control.device.deviceId == m_PlayerInfos.DeviceID)
        {
            if (p_Context.performed)
            {
                m_CharacterInfos.IsShielding = true;
            }
            else if (p_Context.canceled)
            {
                m_CharacterInfos.IsShielding = false;
            }
        }
    }
    public void ShieldDirectionInput(InputAction.CallbackContext p_Context)
    {
        if (p_Context.control.device.deviceId == m_PlayerInfos.DeviceID)
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

    public void TakeShieldDamages(SO_HitBox p_Hitbox)
    {
        
    }
    public void TakeShieldDamages(SO_Projectile p_Projectile)
    {

    }
}

public enum ShieldState
{
    NoShield,
    Right,
    UpRight,
    Up,
    UpLeft,
    Left,
    Omni,
}

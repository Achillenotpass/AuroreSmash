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

    
    public void CustomUpdate(float p_DeltaTime)
    {
        
    }
    public void ShieldInput(InputAction.CallbackContext p_Context)
    {
        if (p_Context.performed)
        {
            m_CurrentShieldState = GetShieldState(m_CurrentShieldDirection);
            SetShieldRotation(m_CurrentShieldState);
        }
        else if (p_Context.canceled)
        {
            m_CurrentShieldState = ShieldState.NoShield;
            SetShieldRotation(m_CurrentShieldState);
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
        m_CurrentShieldState = GetShieldState(m_CurrentShieldDirection);
        SetShieldRotation(m_CurrentShieldState);
    }
    private ShieldState GetShieldState(Vector2 p_ShieldInputDirection)
    {
        if (p_ShieldInputDirection.magnitude > 0.2f)
        {
            float l_Angle = Vector2.Angle(Vector2.right, p_ShieldInputDirection.normalized);
            if (p_ShieldInputDirection.y < 0 && l_Angle > 40.0f && l_Angle < 140.0f)
            {
                return ShieldState.Omni;
            }
            else if (l_Angle <= 40.0f)
            {
                return ShieldState.Right;
            }
            else if(l_Angle > 140.0f)
            {
                return ShieldState.Left;
            }
            else if (l_Angle <= 75.0f)
            {
                return ShieldState.UpRight;
            }
            else if (l_Angle <= 105.0f)
            {
                return ShieldState.Up;
            }
            else if (l_Angle <= 140.0f)
            {
                return ShieldState.UpLeft;
            }
        }
        return ShieldState.Right;
    }
    private void SetShieldRotation(ShieldState p_ShieldState)
    {
        switch (p_ShieldState)
        {
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
        m_ShieldPositions[p_ShieldPart].SetActive(true);
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

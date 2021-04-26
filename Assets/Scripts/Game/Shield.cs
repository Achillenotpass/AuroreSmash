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

    private ShieldState m_CurrentShieldState = ShieldState.NoShield;
    [SerializeField]
    private int m_LagBefore = 1;
    [SerializeField]
    private int m_LagAfter = 1;
    [SerializeField]
    private int m_LagAfterOmni = 1;

    public void ShieldInput(InputAction.CallbackContext p_Context)
    {
        if (p_Context.performed)
        {

        }
        else if (p_Context.canceled)
        {
            m_CurrentShieldState = ShieldState.NoShield;
        }
    }
    public void ShieldDirectionInput(InputAction.CallbackContext p_Context)
    {

    }

    public void CustomUpdate(float p_DeltaTime)
    {
        throw new System.NotImplementedException();
    }
}

public enum ShieldState
{
    NoShield,
    Directional,
    Omnipotent,
}

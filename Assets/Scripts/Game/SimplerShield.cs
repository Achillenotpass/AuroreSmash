using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class SimplerShield : MonoBehaviour, IUpdateUser
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
    [SerializeField]
    private float m_DamageReduction = 80.0f;
    [SerializeField]
    private int m_LagBefore = 10;
    [SerializeField]
    private int m_LagAfter = 10;
    private int m_CurrentFrameCount = 0;
    [SerializeField]
    private GameObject m_ShieldObject = null;
    private CharacterInfos m_CharacterInfos = null;
    private CharacterMovement m_CharacterMovement = null;
    private ESubShieldState m_SubShieldState = ESubShieldState.BeforeLag;
    [SerializeField]
    private UnityEvent m_BlockEvent = null;

    #endregion

    #region Awake/Start/Update
    private void Awake()
    {
        m_CharacterInfos = GetComponent<CharacterInfos>();
        m_CharacterMovement = GetComponent<CharacterMovement>();
    }
    public void CustomUpdate(float p_DeltaTime)
    {
        if (m_CharacterInfos.CurrentCharacterState == CharacterState.Shielding)
        {
            CalculateShieldFrames(p_DeltaTime);
        }
    }
    #endregion

    #region Inputs
    public void ShieldInput(InputAction.CallbackContext p_Context)
    {
        if (p_Context.started)
        {
            m_CharacterMovement.TerminateMomentum();
            m_CurrentFrameCount = 0;
            m_CharacterInfos.CurrentCharacterState = CharacterState.Shielding;
            m_SubShieldState = ESubShieldState.BeforeLag;
        }
        else if (p_Context.canceled)
        {
            m_CurrentFrameCount = 0;
            m_SubShieldState = ESubShieldState.AfterLag;
            DeactivateShield();
        }
    }
    #endregion

    #region Functions
    private void CalculateShieldFrames(float p_DeltaTime)
    {
        if (m_CharacterInfos.CurrentCharacterState == CharacterState.Shielding)
        {
            Debug.Log("oui");
            switch (m_SubShieldState)
            {
                case ESubShieldState.BeforeLag:
                    m_CurrentFrameCount = m_CurrentFrameCount + 1;
                    if (m_CurrentFrameCount >= m_LagBefore)
                    {
                        m_SubShieldState = ESubShieldState.Shielding;
                        ActivateShield();
                    }
                    break;
                case ESubShieldState.Shielding:
                    break;
                case ESubShieldState.AfterLag:
                    m_CurrentFrameCount = m_CurrentFrameCount + 1;
                    if (m_CurrentFrameCount >= m_LagAfter)
                    {
                        m_SubShieldState = ESubShieldState.BeforeLag;
                        m_CharacterInfos.CurrentCharacterState = CharacterState.Idle;
                    }
                    break;
            }
        }
    }
    private void ActivateShield()
    {
        m_ShieldObject.SetActive(true);
    }
    private void DeactivateShield()
    {
        m_ShieldObject.SetActive(false);
    }
    public void TakeShieldDamages(SO_HitBox p_Hitbox, Vector3 p_AttackerPosition)
    {
        SO_HitBox l_ShieldedHitBox = new SO_HitBox();
        l_ShieldedHitBox.Damages = p_Hitbox.Damages * (1.0f - m_DamageReduction / 100.0f);
        l_ShieldedHitBox.EjectionPower = 0.0f;
        GetComponent<Health>().TakeDamages(l_ShieldedHitBox, p_AttackerPosition);

        m_BlockEvent.Invoke();
    }
    public void TakeShieldDamages(SO_Projectile p_Projectile, Vector3 p_ProjectileObjectPosition)
    {
        SO_Projectile l_ShieldedProjectile = new SO_Projectile();
        l_ShieldedProjectile.Damages = p_Projectile.Damages * (1.0f - m_DamageReduction / 100.0f);
        l_ShieldedProjectile.EjectionPower = 0.0f;
        GetComponent<Health>().TakeDamages(l_ShieldedProjectile, p_ProjectileObjectPosition);

        m_BlockEvent.Invoke();
    }
    #endregion

    private enum ESubShieldState
    {
        BeforeLag,
        Shielding,
        AfterLag,
    }
}

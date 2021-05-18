using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private int m_MaxLives = 3;
    public int MaxLives { get { return m_MaxLives; } }
    [SerializeField]
    private int m_CurrentLives = 3;
    public int CurrentLives { get { return m_CurrentLives; } set { m_CurrentLives = Mathf.Clamp(value, 0, m_MaxLives); } }

    [SerializeField]
    private float m_MaxHealth = 100.0f;
    [SerializeField]
    private float m_CurrentHealth = 100.0f;
    private CharacterInfos m_CharacterInfos = null;
    #endregion

    private void Awake()
    {
        m_CharacterInfos = GetComponent<CharacterInfos>();
    }
    private void Start()
    {
        m_CurrentHealth = m_MaxHealth;
        m_CurrentLives = m_MaxLives;
    }


    #region Functions
    public void TakeDamages(SO_HitBox p_HitBox)
    {
        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth - p_HitBox.Damages, 0.0f, m_MaxHealth);

        m_CharacterInfos.CurrentCharacterState = CharacterState.Hitlag;
        Invoke(nameof(StopHitLag), p_HitBox.HitLag);
        GetComponent<MeshRenderer>().enabled = true;
    }
    public void TakeDamages(SO_Projectile p_Projectile)
    {
        m_CharacterInfos.CurrentCharacterState = CharacterState.Hitlag;
        Invoke(nameof(StopHitLag), p_Projectile.HitLag);
        GetComponent<MeshRenderer>().enabled = true;
    }
    public void StopHitLag()
    {
        m_CharacterInfos.CurrentCharacterState = CharacterState.Idle;
        GetComponent<MeshRenderer>().enabled = false;
    }
    public void LoseLife()
    {
        CurrentLives = CurrentLives - 1;
    }
    #endregion

    public float MaxHealth
    {
        get { return m_MaxHealth; }
    }

    public float CurrentHealth
    {
        get { return m_CurrentHealth; }
    }
}
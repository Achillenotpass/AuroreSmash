using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    //Feedbacks
    private Slider m_HealthBar = null;
    public Slider HealthBar { set { m_HealthBar = value; } }
    #endregion

    #region Awake/Start/Update
    private void Awake()
    {
        m_CharacterInfos = GetComponent<CharacterInfos>();
    }
    private void Start()
    {
        m_CurrentHealth = m_MaxHealth;
        m_CurrentLives = m_MaxLives;
    }
    private void Update()
    {
        if (m_HealthBar != null)
        {
            m_HealthBar.value = m_CurrentHealth;
        }
    }
    #endregion


    #region Functions
    public void TakeDamages(SO_HitBox p_HitBox, GameObject p_Attacker)
    {
        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth - p_HitBox.Damages, 0.0f, m_MaxHealth);

        //EJECTION DE QUAND ON TOMBE A 0 PVS
        if (m_CurrentHealth <= 0.0f)
        {
            p_HitBox.EjectionPower = 100000.0f;
            Death();
        }

        if(p_HitBox.EjectionPower > 0.0f)
        {
            m_CharacterInfos.CurrentCharacterState = CharacterState.Hitlag;
            Invoke(nameof(StopHitLag), p_HitBox.HitLag);
            GetComponent<MeshRenderer>().enabled = true;
            //On appelle la fonction apliquant l'éjection sur le joueur touché 
            GetComponent<CharacterEjection>().Ejection(p_HitBox.EjectionPower, p_HitBox.EjectionAngle, p_Attacker);
        }
    }
    public void TakeDamages(SO_Projectile p_Projectile, GameObject p_ProjectileObject)
    {
        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth - p_Projectile.Damages, 0.0f, m_MaxHealth);

        //EJECTION DE QUAND ON TOMBE A 0 PVS
        if (m_CurrentHealth <= 0.0f)
        {
            p_Projectile.EjectionPower = 100000.0f;
            Death();
        }

        if (p_Projectile.EjectionPower > 0.0f)
        {
            m_CharacterInfos.CurrentCharacterState = CharacterState.Hitlag;
            Invoke(nameof(StopHitLag), p_Projectile.HitLag);
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<CharacterEjection>().Ejection(p_Projectile.EjectionPower, p_Projectile.EjectionAngle, p_ProjectileObject);
        }
    }
    public void Death()
    {
        StartCoroutine(FindObjectOfType<Camera>().gameObject.GetComponent<Shake>().CreateShake(0.3f, 0.7f));
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
    [ContextMenu("Lose 10 HPs")]
    public void TakeDamagesDebug()
    {
        CurrentHealth = CurrentHealth - 10.0f;
    }
    #endregion

    public float MaxHealth
    {
        get { return m_MaxHealth; }
    }

    public float CurrentHealth
    {
        get { return m_CurrentHealth; }
        set { m_CurrentHealth = value; }
    }
}
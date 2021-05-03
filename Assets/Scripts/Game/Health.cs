using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    #region Variables
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
    }


    #region Functions
    public void TakeDamages(SO_HitBox p_HitBox)
    {
        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth - p_HitBox.Damages, 0.0f, m_MaxHealth);

        m_CharacterInfos.IsHitLagging = true;
        Invoke(nameof(StopHitLag), p_HitBox.HitLag);
        GetComponent<MeshRenderer>().enabled = true;
    }
    public void TakeDamages(SO_Projectile p_Projectile)
    {
        m_CharacterInfos.IsHitLagging = true;
        Invoke(nameof(StopHitLag), p_Projectile.HitLag);
        GetComponent<MeshRenderer>().enabled = true;
    }
    public void StopHitLag()
    {
        m_CharacterInfos.IsHitLagging = false;
        GetComponent<MeshRenderer>().enabled = false;
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
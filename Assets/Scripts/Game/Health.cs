using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private float m_MaxHealth = 100.0f;
    private float m_CurrentHealth = 100.0f;
    #endregion

    private void Start()
    {
        m_CurrentHealth = m_MaxHealth;
    }


    #region Functions
    public void TakeDamages(float p_Damages)
    {
        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth - p_Damages, 0.0f, m_MaxHealth);
        Debug.Log(m_CurrentHealth);
    }
    #endregion
}

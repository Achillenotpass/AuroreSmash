using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPlayTest : MonoBehaviour, IUpdateUser
{
    [SerializeField]
    private SO_Projectile m_Projectile = null;
    [SerializeField]
    private List<GameObject> m_SpawnPoints = null;
    private CharacterInfos m_Player = null;
    [SerializeField]
    private float m_TimeBetweenProjectile = 2.5f;
    private float m_CurrentTime = 0.0f;

    private void Awake()
    {
        m_Player = FindObjectOfType<CharacterInfos>();
    }
    void IUpdateUser.CustomUpdate(float p_DeltaTime)
    {
        m_CurrentTime = m_CurrentTime + p_DeltaTime;

        if (m_CurrentTime >= m_TimeBetweenProjectile)
        {
            FireProjectile(m_Projectile);
            m_CurrentTime = 0.0f;
        }
    }

    private void FireProjectile(SO_Projectile p_Projectile)
    {

    }
}

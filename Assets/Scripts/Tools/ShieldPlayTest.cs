using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPlayTest : MonoBehaviour, IUpdateUser
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
    [SerializeField]
    private SO_Projectile m_Projectile = null;
    [SerializeField]
    private List<GameObject> m_SpawnPoints = null;
    private CharacterInfos m_Player = null;
    [SerializeField]
    private float m_TimeBetweenProjectile = 2.5f;
    private float m_CurrentTime = -3.0f;
    [SerializeField]
    private LayerMask m_AttackableLayer;

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
        GameObject l_Projectile = Instantiate(p_Projectile.ProjectilePrefab);
        int l_SpawnPoint = Random.Range(0, m_SpawnPoints.Count);
        l_Projectile.transform.position = m_SpawnPoints[l_SpawnPoint].transform.position;
        Vector3 l_Direction = m_Player.transform.position - m_SpawnPoints[l_SpawnPoint].transform.position;
        float l_Angle = Vector3.Angle(Vector3.right, l_Direction);
        l_Angle = l_Angle * Mathf.Sign(l_Direction.y);
        l_Projectile.transform.Rotate(Vector3.forward, l_Angle);
        l_Projectile.GetComponent<Projectile>().ProjectileStats = p_Projectile;
        l_Projectile.GetComponent<Projectile>().AttackableLayer = m_AttackableLayer;
    }
}

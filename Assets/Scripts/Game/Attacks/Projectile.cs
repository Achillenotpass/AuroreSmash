using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IUpdateUser
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
    private SO_Projectile m_ProjectileStats = null;
    public SO_Projectile ProjectileStats { get { return m_ProjectileStats; } set { m_ProjectileStats = value; } }
    [SerializeField]
    private float m_ColliderRadius = 1.0f;
    [SerializeField]
    private LayerMask m_AttackableLayers = 0;
    public LayerMask AttackableLayer { get { return m_AttackableLayers; } set { m_AttackableLayers = value; } }
    private Collider[] m_HitObjects = null;
    #endregion
    #region Awake/Start/update
    public Projectile (LayerMask p_AttackableLayers, SO_Projectile p_ProjectileStats)
    {
        m_AttackableLayers = p_AttackableLayers;
        m_ProjectileStats = p_ProjectileStats;
    }
    public void CustomUpdate(float p_DeltaTime)
    {
        m_HitObjects = Physics.OverlapSphere(transform.position, m_ColliderRadius, m_AttackableLayers);
        if (m_HitObjects != null)
        {
            foreach (Collider l_HitObject in m_HitObjects)
            {
                if(l_HitObject.gameObject.tag == "Shield")
                {
                    Shield l_HitShield = l_HitObject.GetComponentInParent<Shield>();
                    if (l_HitShield != null)
                    {
                        l_HitShield.TakeShieldDamages(m_ProjectileStats, this.gameObject);
                        Destroy(gameObject);
                        return;
                    }
                }
                if (l_HitObject.GetComponent<Health>())
                {
                    l_HitObject.GetComponent<Health>().TakeDamages(m_ProjectileStats, this.gameObject);
                    Destroy(gameObject);
                    break;
                }
            }
        }
        transform.Translate(transform.right * p_DeltaTime * m_ProjectileStats.Speed, Space.World);
    }
    #endregion

}

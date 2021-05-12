using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeLosux : MonoBehaviour
{
    private List<Health> m_Characters = new List<Health>();
    public void Register(Health p_Health)
    {
        m_Characters.Add(p_Health);
    }
    public void UnRegister(Health p_Health)
    {
        m_Characters.Remove(p_Health);
    }

    [SerializeField]
    private Vector2 m_MapCenterPosition = Vector2.zero;
    [SerializeField]
    private Vector2 m_MapSize = Vector2.one;

    private void Update()
    {
        foreach (Health l_health in m_Characters)
        {
            if (l_health.transform.position.x >= m_MapCenterPosition.x + m_MapSize.x / 2
                || l_health.transform.position.x <= m_MapCenterPosition.x - m_MapSize.x / 2
                || l_health.transform.position.y >= m_MapCenterPosition.y + m_MapSize.y / 2
                || l_health.transform.position.y <= m_MapCenterPosition.y - m_MapSize.y / 2)
            {
                //Faire perdre une vie au joueur
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(m_MapCenterPosition.x, m_MapCenterPosition.y, 0), new Vector3(m_MapSize.x, m_MapSize.y, 0));
    }
}

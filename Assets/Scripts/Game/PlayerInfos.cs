using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInfos : MonoBehaviour
{
    private int m_PlayerIndex = 1;
    public int PlayerIndex { get { return m_PlayerIndex; }set { m_PlayerIndex = value; } }
    [SerializeField]
    private LayerMask m_AttackableLayers;
    public LayerMask AttackableLayers { get { return m_AttackableLayers; } set { m_AttackableLayers = value; } }
}

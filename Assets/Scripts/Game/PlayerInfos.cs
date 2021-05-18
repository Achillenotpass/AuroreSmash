using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInfos : MonoBehaviour
{
    [SerializeField]
    private string m_PlayerName = "Naruto";
    public string PlayerName { get { return m_PlayerName; } }
    private bool m_HastLost = false;
    public bool HasLost { get { return m_HastLost; } }
    [SerializeField]
    private LayerMask m_AttackableLayers;
    public LayerMask AttackableLayers { get { return m_AttackableLayers; } }
}

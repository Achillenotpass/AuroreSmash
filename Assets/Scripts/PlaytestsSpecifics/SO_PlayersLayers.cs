using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLayers", menuName = "ScriptableObjects/Gameplay/NewLayers")]
public class SO_PlayersLayers : ScriptableObject
{
    [SerializeField]
    private LayerMask m_PlayerLayer;
    public LayerMask PlayerLayer { get { return m_PlayerLayer; } }

    [SerializeField]
    private LayerMask m_AttackableLayer;
    public LayerMask AttackableLayer { get { return m_AttackableLayer; } }
}

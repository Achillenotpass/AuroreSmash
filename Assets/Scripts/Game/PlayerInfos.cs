using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInfos : MonoBehaviour
{
    [SerializeField]
    private LayerMask m_AttackableLayers;
    public LayerMask AttackableLayers { get { return m_AttackableLayers; } }
}

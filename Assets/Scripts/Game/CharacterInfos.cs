using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfos : MonoBehaviour
{
    private bool m_IsAttacking = false;
    public bool IsAttacking { get { return m_IsAttacking; } set { m_IsAttacking = value; } }
    private bool m_IsHitLagging = false;
    public bool IsHitLagging { get { return m_IsAttacking; } set { m_IsHitLagging = value; } }
}

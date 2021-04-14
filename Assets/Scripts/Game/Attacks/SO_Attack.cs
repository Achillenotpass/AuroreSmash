using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "ScriptableObjects/Gameplay/Attacks/NewAttack")]
public class SO_Attack : ScriptableObject
{
    [SerializeField]
    private int m_AfterLag = 1;
    public int AfterLag { get { return m_AfterLag; } }
    [SerializeField]
    private List<SO_Hit> m_Hit = new List<SO_Hit>();
    public List<SO_Hit> Hit { get { return m_Hit; } }
}

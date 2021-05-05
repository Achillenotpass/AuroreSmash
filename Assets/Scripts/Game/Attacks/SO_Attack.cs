using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "ScriptableObjects/Gameplay/Attacks/NewAttack")]
public class SO_Attack : ScriptableObject
{
    [SerializeField]
    //Represents the time after the attack where you can't attack
    private int m_AfterLag = 1;
    public int AfterLag { get { return m_AfterLag; } }
    //The attack is divided in different hits. A player can be hit by different hit, but only once per hit.
    [SerializeField]
    private List<SO_Hit> m_Hits = new List<SO_Hit>();
    public List<SO_Hit> Hits { get { return m_Hits; } }
    [SerializeField]
    private List<SO_Projectile> m_Projectiles = new List<SO_Projectile>();
    public List<SO_Projectile> Projectiles { get { return m_Projectiles; } }
    [SerializeField]
    private SO_Attack m_Combo = null;
    public SO_Attack Combo { get { return m_Combo; } }
    [SerializeField]
    private AnimationCurve m_PlayerInfluenceOnSpeed = new AnimationCurve();
    public AnimationCurve PlayerInfluenceOnSpeed { get { return m_PlayerInfluenceOnSpeed; } }
    [SerializeField]
    private AnimationCurve m_CharacterXMovement = new AnimationCurve();
    public AnimationCurve CharacterXMovement { get { return m_CharacterXMovement; } }
    [SerializeField]
    private AnimationCurve m_CharacterYMovement = new AnimationCurve();
    public AnimationCurve CharacterYMovement { get { return m_CharacterYMovement; } }
}

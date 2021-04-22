using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHit", menuName = "ScriptableObjects/Gameplay/Attacks/NewHit")]
public class SO_Hit : ScriptableObject
{
    //A hit can have one, or multiple hitboxes. A player can only be hit per one hitbox per hit.
    [SerializeField]
    private List<SO_HitBox> m_HitBoxes = new List<SO_HitBox>();
    public List<SO_HitBox> HitBoxes { get { return m_HitBoxes; } }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "ScriptableObjects/Gameplay/NewCharacter")]
public class SO_Character : ScriptableObject
{
    [SerializeField]
    private GameObject m_CharacterPrefab = null;
    public GameObject CharacterPrefab { get { return m_CharacterPrefab; } }
}

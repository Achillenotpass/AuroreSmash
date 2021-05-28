using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "ScriptableObjects/Gameplay/NewCharacter")]
public class SO_Character : ScriptableObject
{
    [SerializeField]
    private GameObject m_CharacterPrefab = null;
    public GameObject CharacterPrefab { get { return m_CharacterPrefab; } }
    [SerializeField]
    private CharacterSelectionData m_CharacterSelectionDatas = null;
    public CharacterSelectionData CharacterSelectionDatas { get { return m_CharacterSelectionDatas; } }
    [SerializeField]
    private HealthBarData m_HealthBarDatas = null;
    public HealthBarData HealthBarDatas { get { return m_HealthBarDatas; } }


    [System.Serializable]
    public class CharacterSelectionData
    {
        public Sprite m_SelectionSprite = null;
        public Sprite m_InGameSprite = null;
    }
    [System.Serializable]
    public class HealthBarData
    {
        public Sprite m_HealthBarImage = null;
        public Sprite m_HealthBarLogo = null;
        public Sprite m_HealtBarNameHolder = null;
    }
}

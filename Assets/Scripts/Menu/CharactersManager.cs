using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharactersManager : MonoBehaviour
{
    #region Variables
    //Characters datas
    [SerializeField]
    private List<SO_Character> m_AvailableCharacters = new List<SO_Character>();
    //Selectors datas
    private List<CharacterSelector> m_Selectors = new List<CharacterSelector>();
    //Display
    [SerializeField]
    private List<CharacterDisplay> m_CharactersDisplay = null;
    [SerializeField]
    private List<Text> m_CharactersName;
    private int m_CurrentPlayerIndex = 1;
    #endregion

    #region Awake/Start/Update
    private void Start()
    {
        UsersManager.UnRegisterAllUser();
    }
    #endregion

    #region Functions
    public void Register(CharacterSelector p_Selector)
    {
        m_Selectors.Add(p_Selector);
        p_Selector.SelectorUserInfos.m_PlayerIndex = m_CurrentPlayerIndex;
        m_CurrentPlayerIndex = m_CurrentPlayerIndex + 1;
    }
    public SO_Character GetRandomCharacter(CharacterSelector p_Selector)
    {
        int l_SelectorIndex = m_Selectors.IndexOf(p_Selector);
        SO_Character l_NewCharacter = m_AvailableCharacters[Random.Range(0, m_AvailableCharacters.Count)];
        UpdateDisplay(l_SelectorIndex, l_NewCharacter);
        return l_NewCharacter;
    }
    public SO_Character ChangeCharacter(SO_Character p_CurrentCharacter, CharacterSelector p_Selector)
    {
        SO_Character l_NewCharacter = null;
        int l_SelectorIndex = m_Selectors.IndexOf(p_Selector);
        if (p_CurrentCharacter == null)
        {
            l_NewCharacter = m_AvailableCharacters[0];
            UpdateDisplay(l_SelectorIndex, l_NewCharacter);
            m_CharactersName[l_SelectorIndex].text = l_NewCharacter.name;
            return l_NewCharacter;
        }
        else
        {
            int l_CurrentIndex = m_AvailableCharacters.IndexOf(p_CurrentCharacter);
            if (l_CurrentIndex == m_AvailableCharacters.Count - 1)
            {
                l_NewCharacter = m_AvailableCharacters[0];
                UpdateDisplay(l_SelectorIndex, l_NewCharacter);
                return l_NewCharacter;
            }
            else
            {
                l_NewCharacter = m_AvailableCharacters[l_CurrentIndex + 1];
                UpdateDisplay(l_SelectorIndex, l_NewCharacter);
                return l_NewCharacter;
            }

        }
    }
    private void UpdateDisplay(int p_Index, SO_Character p_Character)
    {
        if (p_Character != null)
        {
            m_CharactersDisplay[p_Index].m_PressStart.enabled = false;
            m_CharactersDisplay[p_Index].m_CharacterImage.gameObject.SetActive(true);
            m_CharactersDisplay[p_Index].m_CharacterImage.sprite = p_Character.CharacterSelectionDatas.m_SelectionSprite;
            m_CharactersDisplay[p_Index].m_CharacterName.text = p_Character.CharacterName;
        }
        else
        {
            m_CharactersDisplay[p_Index].m_PressStart.enabled = true;
            m_CharactersDisplay[p_Index].m_CharacterImage.gameObject.SetActive(false);
            m_CharactersDisplay[p_Index].m_CharacterImage.sprite = null;
            m_CharactersDisplay[p_Index].m_CharacterName.text = p_Character.CharacterName;
        }
    }
    public void SelectionDisplay(CharacterSelector p_Selector, SO_Character p_Character, bool p_Selected)
    {
        int l_SelectorIndex = m_Selectors.IndexOf(p_Selector);
        if (p_Selected)
        {
            m_CharactersDisplay[l_SelectorIndex].m_InGameCharacterImage.sprite = p_Character.CharacterSelectionDatas.m_InGameSprite;
            m_CharactersDisplay[l_SelectorIndex].m_InGameCharacterImage.gameObject.SetActive(true);
        }
        else
        {
            m_CharactersDisplay[l_SelectorIndex].m_InGameCharacterImage.gameObject.SetActive(false);
        }
    }
    public void StartGame()
    {
        if (m_Selectors.Count < 2)
        {
            return;
        }
        //On vérifie d'abord si tous les joueurs ont sélectionné leur personnage
        foreach (CharacterSelector l_Selector in m_Selectors)
        {
            if (l_Selector.SelectorUserInfos.UserCharacter == null)
            {
                return;
            }
        }

        //Puis on vide la liste d'utilisateurs
        UsersManager.UnRegisterAllUser();
        //Avant de la remplir avec les nouveaux joueurs
        foreach (CharacterSelector l_Selector in m_Selectors)
        {
            UsersManager.RegisterUser(l_Selector.SelectorUserInfos);
        }

        //Puis on va sur la scène de combat
        SceneManager.LoadScene("MapSelection");
    }
    #endregion

    [System.Serializable]
    private class CharacterDisplay
    {
        public Text m_PressStart = null;
        public Text m_CharacterName = null;
        public Image m_CharacterImage = null;
        public Image m_InGameCharacterImage = null;
    }
}

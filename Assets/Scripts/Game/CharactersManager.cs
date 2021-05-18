using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersManager : MonoBehaviour
{
    #region Variables
    //Characters datas
    [SerializeField]
    private List<SO_Character> m_AvailableCharacters = new List<SO_Character>();
    //Selectors datas
    private List<CharacterSelector> m_Selectors = new List<CharacterSelector>();
    private UsersManager m_UsersManager = null;
    //Display
    #endregion

    #region Awake/Start/Update
    private void Awake()
    {
        m_UsersManager = FindObjectOfType<UsersManager>();
    }
    #endregion

    #region Functions
    public void Register(CharacterSelector p_Selector)
    {
        m_Selectors.Add(p_Selector);
    }
    public SO_Character GetRandomCharacter()
    {
        return m_AvailableCharacters[Random.Range(0, m_AvailableCharacters.Count)];
    }
    public SO_Character ChangeCharacter(SO_Character p_CurrentCharacter)
    {
        if (p_CurrentCharacter == null)
        {
            return m_AvailableCharacters[0];
        }
        else
        {
            int l_CurrentIndex = m_AvailableCharacters.IndexOf(p_CurrentCharacter);
            if (l_CurrentIndex == m_AvailableCharacters.Count - 1)
            {
                return m_AvailableCharacters[0];
            }
            else
            {
                return m_AvailableCharacters[l_CurrentIndex + 1];
            }

        }
    }
    public void StartGame()
    {
        foreach (CharacterSelector l_Selector in m_Selectors)
        {
            if (l_Selector.SelectorUserInfos.UserCharacter == null)
            {
                return;
            }
        }
        foreach (CharacterSelector l_Selector in m_Selectors)
        {
            m_UsersManager.RegisterUser(l_Selector.SelectorUserInfos);
        }
    }
    #endregion
}

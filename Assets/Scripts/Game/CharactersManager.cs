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
    //FightDatas
    [SerializeField]
    private string m_FightScene;
    //Display
    [SerializeField]
    private List<Text> m_CharactersName;
    #endregion

    #region Awake/Start/Update
    #endregion

    #region Functions
    public void Register(CharacterSelector p_Selector)
    {
        m_Selectors.Add(p_Selector);
    }
    public SO_Character GetRandomCharacter(CharacterSelector p_Selector)
    {
        int l_SelectorIndex = m_Selectors.IndexOf(p_Selector);
        SO_Character l_CurrentCharacter = m_AvailableCharacters[Random.Range(0, m_AvailableCharacters.Count)];
        m_CharactersName[l_SelectorIndex].text = l_CurrentCharacter.name;
        return l_CurrentCharacter;
    }
    public SO_Character ChangeCharacter(SO_Character p_CurrentCharacter, CharacterSelector p_Selector)
    {
        SO_Character l_NewCharacter = null;
        if (p_CurrentCharacter == null)
        {
            l_NewCharacter = m_AvailableCharacters[0];
            m_CharactersName[m_Selectors.IndexOf(p_Selector)].text = l_NewCharacter.name;
            return l_NewCharacter;
        }
        else
        {
            int l_CurrentIndex = m_AvailableCharacters.IndexOf(p_CurrentCharacter);
            if (l_CurrentIndex == m_AvailableCharacters.Count - 1)
            {
                l_NewCharacter = m_AvailableCharacters[0];
                m_CharactersName[m_Selectors.IndexOf(p_Selector)].text = l_NewCharacter.name;
                return l_NewCharacter;
            }
            else
            {
                l_NewCharacter = m_AvailableCharacters[l_CurrentIndex + 1];
                m_CharactersName[m_Selectors.IndexOf(p_Selector)].text = l_NewCharacter.name;
                return l_NewCharacter;
            }

        }
    }
    public void StartGame()
    {

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
        SceneManager.LoadScene(m_FightScene);
    }
    #endregion
}

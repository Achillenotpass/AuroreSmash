using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    #endregion

    #region Awake/Start/Update
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
        Debug.Log("Start game");

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

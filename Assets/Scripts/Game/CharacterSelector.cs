using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CharacterSelector : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private SO_Character m_CurrentCharacter = null;
    private UserInfos m_UserInfos = new UserInfos();
    public UserInfos SelectorUserInfos { get { return m_UserInfos; } }
    private CharactersManager m_CharacterManager = null;
    #endregion

    #region Awake/Start/Update
    private void Awake()
    {
        m_CharacterManager = FindObjectOfType<CharactersManager>();
        m_CharacterManager.Register(this);
    }
    private void Start()
    {
        m_UserInfos.UserInputDevice = GetComponent<PlayerInput>().devices[0];
        m_CurrentCharacter = m_CharacterManager.GetRandomCharacter();
        Debug.Log("Currently on " + m_CurrentCharacter.name);
    }
    #endregion

    #region Inputs
    public void JoystickInput(InputAction.CallbackContext p_Context)
    {
        if (p_Context.started)
        {
            if (m_UserInfos.UserCharacter == null && m_CurrentCharacter != null && m_CharacterManager != null)
            {
                m_CurrentCharacter = m_CharacterManager.ChangeCharacter(m_CurrentCharacter);
                Debug.Log("Currently on " + m_CurrentCharacter.name);
            }
        }
    }
    public void SelectionButton(InputAction.CallbackContext p_Context)
    {
        if (p_Context.started)
        {
            if (m_UserInfos.UserCharacter == null && m_CurrentCharacter != null && m_CharacterManager != null)
            {
                m_UserInfos.UserCharacter = m_CurrentCharacter;
                Debug.Log("Selected " + m_CurrentCharacter.name);
            }
            else if (m_UserInfos.UserCharacter != null && m_CurrentCharacter != null && m_CharacterManager != null)
            {
                m_CharacterManager.StartGame();
            }
        }
    }
    public void CancelButton(InputAction.CallbackContext p_Context)
    {
        if (p_Context.started)
        {
            if (m_UserInfos.UserCharacter != null && m_CurrentCharacter != null && m_CharacterManager != null)
            {
                m_UserInfos.UserCharacter = null;
                Debug.Log("Unselected character");
                Debug.Log("Currently on " + m_CurrentCharacter.name);
            }
            else if (m_UserInfos.UserCharacter == null && m_CurrentCharacter != null && m_CharacterManager != null)
            {
                //Revenir au menu précédent
                Debug.Log("Retour au menu précédent");
            }
        }
    }
    #endregion
}

[System.Serializable]
public class UserInfos
{
    #region Variables
    private InputDevice m_UserInputDevice = null;
    public InputDevice UserInputDevice { get { return m_UserInputDevice; } set { m_UserInputDevice = value; } }
    [SerializeField]
    private SO_Character m_UserCharacter = null;
    public SO_Character UserCharacter { get { return m_UserCharacter; } set { m_UserCharacter = value; } }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class UsersManager : MonoBehaviour
{
    #region Variables
    static public List<UserInfos> m_UsersInfos = new List<UserInfos>();
    static public UserGameStats m_WinnerCharacter = null;
    static public UserGameStats m_LoserCharacter = null;
    #endregion
    static public void RegisterUser(UserInfos p_UserInfos)
    {
        m_UsersInfos.Add(p_UserInfos);
    }
    static public void UnRegisterAllUser()
    {
        m_UsersInfos.Clear();
    }

    public class UserGameStats
    {
        public SO_Character m_PlayedCharacter = null;
        public int m_RemainingLives = 3;
    }
}

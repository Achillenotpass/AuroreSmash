using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class UsersManager : MonoBehaviour
{
    #region Variables
    static public List<UserInfos> m_UsersInfos = new List<UserInfos>();
    #endregion
    static public void RegisterUser(UserInfos p_UserInfos)
    {
        m_UsersInfos.Add(p_UserInfos);
    }
    static public void UnRegisterAllUser()
    {
        m_UsersInfos.Clear();
    }
}

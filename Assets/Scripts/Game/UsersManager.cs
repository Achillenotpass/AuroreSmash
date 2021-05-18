using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class UsersManager : MonoBehaviour
{
    #region Variables
    private List<UserInfos> m_UsersInfos = new List<UserInfos>();
    public List<UserInfos> UsersInfos { get { return m_UsersInfos; } }
    #endregion

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public void RegisterUser(UserInfos p_UserInfos)
    {
        m_UsersInfos.Add(p_UserInfos);
    }
}

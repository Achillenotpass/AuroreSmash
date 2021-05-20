using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserCharacterSelectionTest : MonoBehaviour
{
    [SerializeField]
    private UserInfos m_UserInfos = new UserInfos();

    [ContextMenu("RegisterUser")]
    public void RegisterUser()
    {
        m_UserInfos.UserInputDevice = GetComponent<PlayerInput>().devices[0];
        FindObjectOfType<UsersManager>().RegisterUser(m_UserInfos);
    }
}

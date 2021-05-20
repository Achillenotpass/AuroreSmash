using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneChangeFunctions : MonoBehaviour
{
    private UsersManager m_UserManager = null;
    private void Start()
    {
        m_UserManager = FindObjectOfType<UsersManager>();
    }
    public void GoToScene(string p_GoToScene)
    {
        Translator[] l_Translators = FindObjectsOfType<Translator>();
        foreach (Translator l_Translator in l_Translators)
        {
            //m_UserManager.Devices.Add(l_Translator.GetComponent<PlayerInput>().user.pairedDevices[0]);
        }
        SceneManager.LoadScene(p_GoToScene);
    }

    [ContextMenu("Change scene")]
    public void GoToTest()
    {
        GoToScene("AchilleInputTests");
    }
}

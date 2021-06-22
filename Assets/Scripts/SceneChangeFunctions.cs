using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneChangeFunctions : MonoBehaviour
{
    [SerializeField]
    private string m_SceneName = string.Empty;

    [ContextMenu("Change scene")]
    public void GoToTest()
    {
        SceneManager.LoadScene(m_SceneName);
    }
}

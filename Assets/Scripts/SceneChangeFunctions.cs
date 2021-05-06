using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeFunctions : MonoBehaviour
{
    public void GoToScene(string p_GoToScene)
    {
        SceneManager.LoadScene(p_GoToScene);
    }
}

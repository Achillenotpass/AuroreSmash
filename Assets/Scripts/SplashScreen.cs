using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public void StartInput(InputAction.CallbackContext p_Context)
    {
        SceneManager.LoadScene("MainMenu");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public void AnyInputPressed(InputAction.CallbackContext p_Context)
    {
        SceneManager.LoadScene("MainMenu");
    }
}

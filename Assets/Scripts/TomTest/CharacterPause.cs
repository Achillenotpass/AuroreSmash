using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterPause : MonoBehaviour
{
    private PauseMenu m_PauseMenu = null;
    private void Awake()
    {
        m_PauseMenu = FindObjectOfType<PauseMenu>();
    }

    public void InputForPause(InputAction.CallbackContext p_Context)
    {
        m_PauseMenu.PauseTheGame(p_Context);
    }

    public void InputForResume(InputAction.CallbackContext p_Context)
    {
        m_PauseMenu.ResumeTheGame(p_Context);
    }

    public void SelectInput(InputAction.CallbackContext p_Context)
    {
        SceneManager.LoadScene("CharacterSelection");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject m_PauseMenu = null;
    private List<PlayerInput> m_PlayersInput = new List<PlayerInput>();
    private Controls m_InputActionControls;
    private float m_Time = 1f;

    private void Start()
    {
        m_InputActionControls = new Controls();
    }
    
    private void Update()
    {
        Time.timeScale = m_Time;
    }

    public void FindPlayersInput()
    {
        m_PlayersInput.Clear();
        m_PlayersInput = new List<PlayerInput>(FindObjectsOfType<PlayerInput>());
    }

    public void PauseTheGame(InputAction.CallbackContext p_Context)
    {
        if (p_Context.started)
        {
            FindPlayersInput();
            m_Time = 0;
            for (int i = 0; i < m_PlayersInput.Count; i++)
            {
                m_PlayersInput[i].currentActionMap = m_InputActionControls.UI;
                m_PlayersInput[i].actions.FindActionMap("Player").Disable();
                m_PlayersInput[i].actions.FindActionMap("UI").Enable();
            }
            if(m_PauseMenu != null)
            {
                m_PauseMenu.SetActive(true);
            }
        }
    }

    public void ResumeTheGame(InputAction.CallbackContext p_Context)
    {
        if (p_Context.started)
        {
            FindPlayersInput();
            m_Time = 1;
            for (int i = 0; i < m_PlayersInput.Count; i++)
            {
                m_PlayersInput[i].currentActionMap = m_InputActionControls.Player;
                m_PlayersInput[i].actions.FindActionMap("UI").Disable();
                m_PlayersInput[i].actions.FindActionMap("Player").Enable();
            }
            if (m_PauseMenu != null)
            {
                m_PauseMenu.SetActive(false);
            }
        }
    }
}
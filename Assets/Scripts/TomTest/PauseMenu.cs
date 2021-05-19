using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    private PlayerInput[] m_PlayersInput = null;
    private Controls m_InputActionControls;
    private float m_Time = 1f;

    private void Start()
    {
        FindPlayersInput();
        m_InputActionControls = new Controls();
    }
    
    private void Update()
    {
        Time.timeScale = m_Time;
        Debug.Log(m_PlayersInput[0].currentActionMap);
    }

    public void FindPlayersInput()
    {
        m_PlayersInput = FindObjectsOfType<PlayerInput>();
    }

    public void PauseTheGame(InputAction.CallbackContext p_Context)
    {
        Debug.Log("aaa");
        if (p_Context.started)
        {
            Debug.Log("bbb");
            m_Time = 0;
            for (int i = 0; i < m_PlayersInput.Length; i++)
            {
                m_PlayersInput[i].currentActionMap = m_InputActionControls.UI;
            }
        }
    }

    public void ResumeTheGame(InputAction.CallbackContext p_Context)
    {
        Debug.Log("ccc");
        if (p_Context.started)
        {
            Debug.Log("ddd");
            m_Time = 1;
            for (int i = 0; i < m_PlayersInput.Length; i++)
            {
                m_PlayersInput[i].currentActionMap = m_InputActionControls.Player;
            }
        }
    }
}

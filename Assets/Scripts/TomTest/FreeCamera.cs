using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FreeCamera : MonoBehaviour
{
    private Vector3 m_LaCameraQuiBouge = Vector3.zero;
    private Vector3 m_LaCameraQuiUp = Vector3.zero;
    private Vector3 m_LaCameraQuiDown = Vector3.zero;

    public void JoystickCamera(InputAction.CallbackContext p_Context)
    {
        m_LaCameraQuiBouge = new Vector3(p_Context.ReadValue<Vector2>().x, 0, p_Context.ReadValue<Vector2>().y);
    }

    public void GoUpCamera(InputAction.CallbackContext p_Context)
    {
        if(p_Context.ReadValueAsButton())
            m_LaCameraQuiUp = new Vector3(0, 0.5f, 0);
        else
            m_LaCameraQuiUp = new Vector3(0, 0, 0);

    }

    public void GoDownCamera(InputAction.CallbackContext p_Context)
    {
        if (p_Context.ReadValueAsButton())
            m_LaCameraQuiDown = new Vector3(0, -0.5f, 0);
        else
            m_LaCameraQuiDown = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        transform.Translate(m_LaCameraQuiBouge + m_LaCameraQuiUp + m_LaCameraQuiDown);
    }
}
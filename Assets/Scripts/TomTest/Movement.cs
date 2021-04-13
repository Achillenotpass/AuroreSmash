using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    private CharacterController m_CharacterController = null;

    private Vector3 m_PlayerDirection = Vector3.zero;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    private void Update()
    {

        m_PlayerDirection += new Vector3(0, -10 * Time.deltaTime, 0);
        m_CharacterController.Move(m_PlayerDirection);
        m_PlayerDirection = Vector3.zero;
    }

    private void PlayerMovement(InputAction.CallbackContext p_Context)
    {
        if(p_Context.started)
        {

        }
        if(p_Context.performed)
        {
            m_PlayerDirection += new Vector3(p_Context.ReadValue<Vector2>().x, p_Context.ReadValue<Vector2>().y, 0);
        }
        if(p_Context.canceled)
        {

        }
    }
}
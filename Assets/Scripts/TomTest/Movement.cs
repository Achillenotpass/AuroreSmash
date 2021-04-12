using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

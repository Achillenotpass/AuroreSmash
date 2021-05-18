using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaytestGamemanager : MonoBehaviour
{
    [SerializeField]
    private Transform m_SpawnPoint = null;
    public void PutPlayerAtGoodPosition(PlayerInput p_PlayerInput)
    {
        p_PlayerInput.GetComponent<CharacterController>().enabled = false;
        p_PlayerInput.transform.position = m_SpawnPoint.position;
        p_PlayerInput.GetComponent<CharacterController>().enabled = true;
        p_PlayerInput.transform.SetParent(m_SpawnPoint);
    }
}

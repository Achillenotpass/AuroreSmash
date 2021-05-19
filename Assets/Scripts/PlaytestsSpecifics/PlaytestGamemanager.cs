using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaytestGamemanager : MonoBehaviour
{
    [SerializeField]
    private Transform m_SpawnPoint = null;
    EPlayTestGameState m_GameState = EPlayTestGameState.WaitingForPlayers;

    private List<PlayerInput> m_Players = new List<PlayerInput>();
    [SerializeField]
    private List<SO_PlayersLayers> m_PlayersLayers = new List<SO_PlayersLayers>();
    public void SetupPlayerJoined(PlayerInput p_PlayerInput)
    {
        p_PlayerInput.GetComponent<CharacterController>().enabled = false;
        p_PlayerInput.transform.position = m_SpawnPoint.position;
        p_PlayerInput.GetComponent<CharacterController>().enabled = true;
        p_PlayerInput.transform.SetParent(m_SpawnPoint);

        m_Players.Add(p_PlayerInput);
    }
    private void SetupPlayersLayers()
    {
        for (int i = 0; i < m_Players.Count; i++)
        {
            m_Players[i].GetComponent<PlayerInfos>().AttackableLayers = m_PlayersLayers[i].AttackableLayer;
            m_Players[i].gameObject.layer = m_PlayersLayers[i].PlayerLayer;
        }
    }

    private void Update()
    {
        if (m_Players.Count == 2 && m_GameState == EPlayTestGameState.WaitingForPlayers)
        {
            m_GameState = EPlayTestGameState.WaitingForStart;
        }
    }

    public void SpaceInput(InputAction.CallbackContext p_Context)
    {
        SetupPlayersLayers();
        if (m_GameState == EPlayTestGameState.WaitingForStart)
        {
            m_GameState = EPlayTestGameState.Running;
        }
    }

    private enum EPlayTestGameState
    {
        WaitingForPlayers,
        WaitingForStart,
        Running,
        Ended,
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlaytestGamemanager : MonoBehaviour
{
    [SerializeField]
    private Vector2 m_MapCenterPosition = Vector2.zero;
    [SerializeField]
    private Vector2 m_MapSize = Vector2.one;

    [SerializeField]
    private Transform m_SpawnPoint = null;
    EPlayTestGameState m_GameState = EPlayTestGameState.WaitingForPlayers;

    private List<PlayerInput> m_Players = new List<PlayerInput>();
    [SerializeField]
    private List<SO_PlayersLayers> m_PlayersLayers = new List<SO_PlayersLayers>();
    [SerializeField]
    private List<Slider> m_HealthBars = new List<Slider>();

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
        PlayersCamera l_Camera = FindObjectOfType<PlayersCamera>();
        for (int i = 0; i < m_Players.Count; i++)
        {
            m_Players[i].GetComponent<PlayerInfos>().AttackableLayers = m_PlayersLayers[i].AttackableLayer;
            m_Players[i].gameObject.layer = m_PlayersLayers[i].PlayerLayer;

            l_Camera.ListOfAllPlayers.Add(m_Players[i].GetComponent<CharacterInfos>());

            m_Players[i].GetComponent<Health>().HealthBar = m_HealthBars[i];
        }
        l_Camera.enabled = true;
    }

    private void Update()
    {
        if (m_Players.Count == 2 && m_GameState == EPlayTestGameState.WaitingForPlayers)
        {
            m_GameState = EPlayTestGameState.WaitingForStart;
        }
        if (m_GameState == EPlayTestGameState.WaitingForStart)
        {
            SetupPlayersLayers();
            m_GameState = EPlayTestGameState.Running;
        }
        foreach (PlayerInput l_Player in m_Players)
        {
            CheckEjection(l_Player.GetComponent<Health>());
        }
    }
    private void CheckEjection(Health p_Character)
    {
        if (p_Character.transform.position.x >= m_MapCenterPosition.x + m_MapSize.x / 2
            || p_Character.transform.position.x <= m_MapCenterPosition.x - m_MapSize.x / 2
            || p_Character.transform.position.y >= m_MapCenterPosition.y + m_MapSize.y / 2
            || p_Character.transform.position.y <= m_MapCenterPosition.y - m_MapSize.y / 2)
        {
            SpawnPlayer(p_Character);
        }
    }
    private void SpawnPlayer(Health p_Character)
    {
        p_Character.CurrentHealth = 100.0f;
        p_Character.GetComponent<CharacterController>().enabled = false;
        p_Character.transform.position = m_SpawnPoint.position;
        p_Character.GetComponent<CharacterController>().enabled = true;
    }

    private enum EPlayTestGameState
    {
        WaitingForPlayers,
        WaitingForStart,
        Running,
        Ended,
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(m_MapCenterPosition.x, m_MapCenterPosition.y, 0), new Vector3(m_MapSize.x, m_MapSize.y, 0));
    }
}

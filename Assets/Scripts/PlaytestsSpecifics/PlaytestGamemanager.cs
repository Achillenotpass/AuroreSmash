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
    //Game
    [SerializeField]
    private float m_GameTimer = 90.0f;
    [SerializeField]
    private float m_StartTimer = 5.0f;
    private float m_CurrentTimer = 0.0f;
    [SerializeField]
    private Text m_TimerUI = null;

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
            foreach (Transform l_Child in m_Players[i].gameObject.GetComponentsInChildren<Transform>())
            {
                l_Child.gameObject.layer = m_PlayersLayers[i].PlayerLayer;
            }

            l_Camera.ListOfAllPlayers.Add(m_Players[i].GetComponent<CharacterInfos>());

            m_Players[i].GetComponent<Health>().HealthBar = m_HealthBars[i];
        }
        l_Camera.enabled = true;

        m_GameState = EPlayTestGameState.Running;
        StartCoroutine(RunTimer(m_GameTimer));
    }

    private void Update()
    {
        if (m_Players.Count == 2 && m_GameState == EPlayTestGameState.WaitingForPlayers)
        {
            m_GameState = EPlayTestGameState.WaitingForStart;
            StartCoroutine(RunTimer(m_StartTimer));
        }
        foreach (PlayerInput l_Player in m_Players)
        {
            CheckEjection(l_Player.GetComponent<Health>());
        }
        if (m_TimerUI != null)
        {
            m_TimerUI.text = ((int)m_CurrentTimer).ToString();
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
            p_Character.LoseLife();
        }
    }
    private void SpawnPlayer(Health p_Character)
    {
        p_Character.CurrentHealth = 100.0f;
        p_Character.GetComponent<CharacterController>().enabled = false;
        p_Character.transform.position = m_SpawnPoint.position;
        p_Character.GetComponent<CharacterController>().enabled = true;
    }

    private IEnumerator RunTimer(float p_TimerDuration)
    {
        m_CurrentTimer = p_TimerDuration;
        while (m_CurrentTimer > 0.0f)
        {
            m_CurrentTimer = m_CurrentTimer - Time.deltaTime;
            yield return null;
        }
        if (m_GameState == EPlayTestGameState.Running)
        {
            m_GameState = EPlayTestGameState.Ended;
            if (m_Players[0].GetComponent<Health>().CurrentLives > m_Players[1].GetComponent<Health>().CurrentLives)
            {
                Debug.Log("PLAYER 1 IS THE WINNER");
                Destroy(m_Players[1].gameObject);
            }
            else if (m_Players[0].GetComponent<Health>().CurrentLives < m_Players[1].GetComponent<Health>().CurrentLives)
            {
                Debug.Log("PLAYER 2 IS THE WINNER");
                PlayersCamera l_Camera = FindObjectOfType<PlayersCamera>();
                l_Camera.ListOfAllPlayers.Remove(m_Players[0].GetComponent<CharacterInfos>());
                Destroy(m_Players[0].gameObject);
                m_Players.Remove(m_Players[0]);
            }
            else
            {
                Debug.Log("IT'S A DRAW");
            }
        }
        if (m_GameState == EPlayTestGameState.WaitingForStart)
        {
            SetupPlayersLayers();
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
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(m_MapCenterPosition.x, m_MapCenterPosition.y, 0), new Vector3(m_MapSize.x, m_MapSize.y, 0));
    }
}

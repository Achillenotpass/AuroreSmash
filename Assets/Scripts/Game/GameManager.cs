using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour, IUpdateUser
{
    #region CustomUpdate
    [SerializeField]
    private SO_UpdateLayerSettings m_UpdateSettings = null;
    private void OnEnable()
    {
        m_UpdateSettings.Bind(this);
    }
    private void OnDisable()
    {
        m_UpdateSettings.Unbind(this);
    }
    #endregion
    #region Variables
    //Users datas
    private UsersManager m_UsersManager = null;

    //Game datas
    private int m_PlayerCount = 2;
    public int PlayerCount { get { return m_PlayerCount; } }
    private List<PlayerInfos> m_PlayersAlive = new List<PlayerInfos>();
    private List<Health> m_Characters = new List<Health>();
    [SerializeField]
    private float m_GameTimer = 60.0f;
    public float GameTimer { get { return m_GameTimer; } set { value = m_GameTimer; } }
    private float m_CurrentGameTimer = 60.0f;
    public float CurrentGameTimer { get { return m_CurrentGameTimer; } }
    private EGameState m_GameState = EGameState.WaitingForStart;
    [SerializeField]
    private Vector2 m_MapCenterPosition = Vector2.zero;
    [SerializeField]
    private Vector2 m_MapSize = Vector2.one;
    [SerializeField]
    private Transform m_PlayersSpawn = null;

    //Events
    [SerializeField]
    private UnityEvent m_LoseLifeEvent = null;
    [SerializeField]
    private UnityEvent m_EndGameEvent = null;
    #endregion

    #region Awake/Start/Update
    private void Awake()
    {
        m_UsersManager = FindObjectOfType<UsersManager>();
    }
    private void Start()
    {
        SetupGame();
    }
    public void CustomUpdate(float p_DeltaTime)
    {
        if (m_GameState == EGameState.Running)
        {
            for (int i = 0; i < m_Characters.Count; i++)
            {
                CheckEjection(m_Characters[i]);
                CheckLives(m_Characters[i]);
            }
        }
        CheckEndGameLives();

        CheckTimer(p_DeltaTime);
    }
    #endregion

    #region Functions
    private void SetupGame()
    {
        foreach (UserInfos l_UserInfos in m_UsersManager.UsersInfos)
        {
            PlayerInfos l_SpawnedPLayer = SpawnPlayer(l_UserInfos);
            m_PlayersAlive.Add(l_SpawnedPLayer);
            m_Characters.Add(l_SpawnedPLayer.GetComponent<Health>());
        }

        m_PlayerCount = m_PlayersAlive.Count;
        StartGame();
    }
    private PlayerInfos SpawnPlayer(UserInfos p_UserInfos)
    {
        PlayerInput l_SpawnedPlayer = PlayerInput.Instantiate(p_UserInfos.UserCharacter.CharacterPrefab, -1, null, -1, p_UserInfos.UserInputDevice);
        //Changer position du joueur
        l_SpawnedPlayer.GetComponent<CharacterController>().enabled = false;
        l_SpawnedPlayer.transform.position = m_PlayersSpawn.position;
        l_SpawnedPlayer.GetComponent<CharacterController>().enabled = true;
        //Changer parent du joueur
        l_SpawnedPlayer.transform.SetParent(m_PlayersSpawn);

        return l_SpawnedPlayer.GetComponent<PlayerInfos>();
    }
    private void StartGame()
    {
        m_CurrentGameTimer = GameTimer;

        m_GameState = EGameState.Running;
    }
    private void CheckEjection(Health p_Character)
    {
        if (p_Character.transform.position.x >= m_MapCenterPosition.x + m_MapSize.x / 2
            || p_Character.transform.position.x <= m_MapCenterPosition.x - m_MapSize.x / 2
            || p_Character.transform.position.y >= m_MapCenterPosition.y + m_MapSize.y / 2
            || p_Character.transform.position.y <= m_MapCenterPosition.y - m_MapSize.y / 2)
        {
            p_Character.LoseLife();
            m_LoseLifeEvent.Invoke();
        }
    }
    private void CheckLives(Health p_Character)
    {
        if (p_Character.CurrentLives <= 0)
        {
            m_Characters.Remove(p_Character);
            m_PlayersAlive.Remove(p_Character.GetComponent<PlayerInfos>());
        }
    }
    private void CheckEndGameLives()
    {
        if (m_PlayersAlive.Count == 1)
        {
            EndGame(m_PlayersAlive[0]);
        }
    }
    private void CheckTimer(float p_DeltaTime)
    {
        m_CurrentGameTimer = m_CurrentGameTimer - p_DeltaTime;
        if (m_CurrentGameTimer <= 0.0f)
        {
            EndGame(m_PlayersAlive[0]);
        }
    }
    private void EndGame(PlayerInfos p_WinnerPlayerInfo)
    {
        m_GameState = EGameState.Ended;
        Debug.Log("Winning player is :" + p_WinnerPlayerInfo.PlayerName);
        m_EndGameEvent.Invoke();
    }
    #endregion


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(m_MapCenterPosition.x, m_MapCenterPosition.y, 0), new Vector3(m_MapSize.x, m_MapSize.y, 0));
    }

    private enum EGameState
    {
        WaitingForStart,
        Running,
        Paused,
        Ended,
    }
}


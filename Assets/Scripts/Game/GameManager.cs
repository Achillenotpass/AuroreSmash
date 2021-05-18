using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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

    private int m_PlayerCount = 2;
    public int PlayerCount { get { return m_PlayerCount; } }
    private List<PlayerInfos> m_PlayersAlive = new List<PlayerInfos>();
    [SerializeField]
    private float m_GameTimer = 60.0f;
    public float GameTimer { get { return m_GameTimer; } set { value = m_GameTimer; } }
    private float m_CurrentGameTimer = 60.0f;
    public float CurrentGameTimer { get { return m_CurrentGameTimer; } }
    [SerializeField]
    private UnityEvent m_LoseLifeEvent = null;
    [SerializeField]
    private UnityEvent m_EndGameEvent = null;
    private List<Health> m_Characters = new List<Health>();
    public void Register(Health p_Health)
    {
        m_Characters.Add(p_Health);
    }
    public void UnRegister(Health p_Health)
    {
        m_Characters.Remove(p_Health);
    }
    [SerializeField]
    private Vector2 m_MapCenterPosition = Vector2.zero;
    [SerializeField]
    private Vector2 m_MapSize = Vector2.one;


    private void Start()
    {
        m_CurrentGameTimer = GameTimer;

        m_PlayersAlive = new List<PlayerInfos>(FindObjectsOfType<PlayerInfos>());
        m_PlayerCount = m_PlayersAlive.Count;
        m_Characters = new List<Health>(FindObjectsOfType<Health>());

        if (m_PlayersAlive.Count != m_Characters.Count)
        {
            Debug.Log("Players and characters aren't in the same quantity");
        }

    }
    public void CustomUpdate(float p_DeltaTime)
    {
        foreach (Health l_Health in m_Characters)
        {
            CheckEjection(l_Health);
            CheckLives(l_Health);
        }
        CheckEndGameLives();

        CheckTimer(p_DeltaTime);
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
        Debug.Log("Winning player is :" + p_WinnerPlayerInfo.PlayerName);
        m_EndGameEvent.Invoke();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(m_MapCenterPosition.x, m_MapCenterPosition.y, 0), new Vector3(m_MapSize.x, m_MapSize.y, 0));
    }
}

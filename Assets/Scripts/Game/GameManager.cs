using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField]
    private List<SO_PlayersLayers> m_PlayersLayers = new List<SO_PlayersLayers>();

    //Feedback
    [SerializeField]
    private Text m_TimerText = null;
    [SerializeField]
    private List<HealthBarGame> m_HealthBars = new List<HealthBarGame>();
    [SerializeField]
    private Text m_VictoryText;

    //Events
    [SerializeField]
    private UnityEvent m_LoseLifeEvent = null;
    [SerializeField]
    private UnityEvent m_EndGameEvent = null;
    #endregion

    #region Awake/Start/Update
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
                //On vérifie si un personnage st éjecté hors de la map
                //Si oui, on lui retire une vie
                CheckEjection(m_Characters[i]);
                //On regarde si un joueur n'a plus de vies
                //Si c'est le cas, on le retire de la partie
                CheckLives(m_Characters[i]);
            }
            //On regarde s'il ne reste plus qu'un joueur en lice
            //Si oui, on finit la partie
            CheckEndGameLives();

            //On fait avancer le timer
            //S'il atteint 0, on finit la partie
            CheckTimer(p_DeltaTime);
        }
    }
    #endregion

    #region Functions
    private void SetupGame()
    {
        //Apparition et placement des joueurs
        foreach (UserInfos l_UserInfos in UsersManager.m_UsersInfos)
        {
            PlayerInfos l_SpawnedPLayer = SpawnPlayer(l_UserInfos);
            m_PlayersAlive.Add(l_SpawnedPLayer);
            m_Characters.Add(l_SpawnedPLayer.GetComponent<Health>());

            LinkHealthBar(l_SpawnedPLayer);
        }

        SetupPlayersLayerAndCamera();
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
        //Setup les données du personnage
        l_SpawnedPlayer.GetComponent<CharacterInfos>().Character = p_UserInfos.UserCharacter;

        return l_SpawnedPlayer.GetComponent<PlayerInfos>();
    }
    private void LinkHealthBar(PlayerInfos p_PlayerInfos)
    {
        for (int i = 0; i < m_HealthBars.Count; i++)
        {
            if (m_HealthBars[i].m_HealthBarSlider.maxValue == 1)
            {
                Health l_PlayerHealth = p_PlayerInfos.GetComponent<Health>();
                Slider l_HealthBar = m_HealthBars[i].m_HealthBarSlider;
                l_PlayerHealth.HealthBar = l_HealthBar;
                l_HealthBar.maxValue = l_PlayerHealth.MaxHealth;
                l_HealthBar.value = l_HealthBar.maxValue;

                SO_Character l_CurrentCharacter = l_PlayerHealth.GetComponent<CharacterInfos>().Character;
                m_HealthBars[i].m_HealthBarImage.sprite = l_CurrentCharacter.HealthBarDatas.m_HealthBarImage;
                m_HealthBars[i].m_HealthBarLogo.sprite = l_CurrentCharacter.HealthBarDatas.m_HealthBarLogo;
                m_HealthBars[i].m_HealtBarNameHolder.sprite = l_CurrentCharacter.HealthBarDatas.m_HealtBarNameHolder;

                break;
            }
            else
            {
                continue;
            }
        }
    }
    private void SetupPlayersLayerAndCamera()
    {
        PlayersCamera l_Camera = FindObjectOfType<PlayersCamera>();
        for (int i = 0; i < m_PlayersAlive.Count; i++)
        {
            m_PlayersAlive[i].AttackableLayers = m_PlayersLayers[i].AttackableLayer;
            m_PlayersAlive[i].gameObject.layer = m_PlayersLayers[i].PlayerLayer;
            foreach (Transform l_Child in m_PlayersAlive[i].gameObject.GetComponentsInChildren(typeof(Transform), true))
            {
                Debug.Log(l_Child.gameObject.name + " : " + l_Child.gameObject.layer);
                l_Child.gameObject.layer = m_PlayersLayers[i].PlayerLayer;
            }
            l_Camera.ListOfAllPlayers.Add(m_PlayersAlive[i].GetComponent<CharacterInfos>());
        }

        l_Camera.enabled = true;
    }
    private void StartGame()
    {
        m_CurrentGameTimer = GameTimer;
        m_TimerText.text = m_CurrentGameTimer.ToString();

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

            //Changer position du joueur
            p_Character.GetComponent<CharacterController>().enabled = false;
            p_Character.transform.position = m_PlayersSpawn.position;
            p_Character.GetComponent<CharacterController>().enabled = true;
        }
    }
    private void CheckLives(Health p_Character)
    {
        if (p_Character.CurrentLives <= 0)
        {
            m_Characters.Remove(p_Character);
            //Ajouter feedback de mort de personnage
            m_PlayersAlive.Remove(p_Character.GetComponent<PlayerInfos>());

            PlayersCamera l_Camera = FindObjectOfType<PlayersCamera>();
            l_Camera.ListOfAllPlayers.Remove(p_Character.GetComponent<CharacterInfos>());

            Destroy(p_Character.gameObject);
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
        m_TimerText.text = ((int)m_CurrentGameTimer).ToString();
        if (m_CurrentGameTimer <= 0.0f)
        {
            //On vérifie les vies de tous les joueurs
            //Celui qui a le plus de vies a gagné
            Health l_HighestLives = null;
            for (int i = 0; i < m_Characters.Count; i++)
            {
                if (i == 0)
                {
                    l_HighestLives = m_Characters[i];
                }
                else
                {
                    if (l_HighestLives.CurrentLives < m_Characters[i].CurrentLives)
                    {
                        l_HighestLives = m_Characters[i];
                    }
                    else if (l_HighestLives.CurrentLives == m_Characters[i].CurrentLives)
                    {
                        if (l_HighestLives.CurrentHealth < m_Characters[i].CurrentHealth)
                        {
                            l_HighestLives = m_Characters[i];
                        }
                        else if (l_HighestLives.CurrentHealth == m_Characters[i].CurrentHealth)
                        {
                            //NE PREND EN COMPTE QU'UNE PARTIE A 2 JOUEURS
                            EndGameDraw();
                        }
                    }
                }
            }
            EndGame(l_HighestLives.GetComponent<PlayerInfos>());
        }
    }
    private void EndGame(PlayerInfos p_WinnerPlayerInfo)
    {
        m_GameState = EGameState.Ended;
        m_VictoryText.text = "Winning player is : " + p_WinnerPlayerInfo.PlayerName;
        m_EndGameEvent.Invoke();
    }
    private void EndGameDraw()
    {
        m_GameState = EGameState.Ended;
        m_VictoryText.text = "IT'S A DRAW";
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
    [System.Serializable]
    private class HealthBarGame
    {
        public Slider m_HealthBarSlider = null;
        public Image m_HealthBarImage = null;
        public Image m_HealthBarLogo = null;
        public Image m_HealtBarNameHolder = null;
    }
}


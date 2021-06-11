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
    [Header("Game datas")]
    [SerializeField]
    private float m_TimeBeforeGameStart = 1.0f;
    private int m_PlayerCount = 2;
    public int PlayerCount { get { return m_PlayerCount; } }
    private List<PlayerInfos> m_PlayersAlive = new List<PlayerInfos>();
    private List<Health> m_Characters = new List<Health>();
    [SerializeField]
    private float m_GameTimer = 60.0f;
    public float GameTimer { get { return m_GameTimer; } set { value = m_GameTimer; } }
    [SerializeField]
    private float m_RespawnDelay = 2.0f;
    private float m_CurrentGameTimer = 60.0f;
    public float CurrentGameTimer { get { return m_CurrentGameTimer; } }
    private EGameState m_GameState = EGameState.WaitingForStart;
    [SerializeField]
    private Vector3 m_MapCenterPosition = Vector2.zero;
    [SerializeField]
    private Vector3 m_MapSize = Vector2.one;
    [SerializeField]
    private Vector3 m_CameraCenterPosition = Vector2.zero;
    public Vector3 CameraCenterPosition { get { return m_CameraCenterPosition; } }
    [SerializeField]
    private Vector3 m_CameraSize = Vector2.one;
    public Vector3 CameraSize { get { return m_CameraSize; } }
    [SerializeField]
    private Transform m_PlayersParent = null;
    [SerializeField]
    private List<Transform> m_PlayersSpawn = new List<Transform>();
    [SerializeField]
    private List<SO_PlayersLayers> m_PlayersLayers = new List<SO_PlayersLayers>();
    private List<int> m_UsedSpawnPoints = new List<int>();
    private AsyncOperation m_VictorySceneAsync = null;
    [SerializeField]
    private GameObject m_CameraRaycastPlane = null;

    [Header("Feedbacks")]
    [SerializeField]
    private Image m_BeginningTimer = null;
    [SerializeField]
    private List<Sprite> m_BeginningTimerSprites = new List<Sprite>();
    [SerializeField]
    private List<Sprite> m_EndTimerSprites = new List<Sprite>();
    [SerializeField]
    private Text m_TimerText = null;
    [SerializeField]
    private List<HealthBarGame> m_HealthBars = new List<HealthBarGame>();
    [SerializeField]
    private Text m_VictoryText;
    [SerializeField]
    private float m_MinimumTimeAfterGame = 2.5f;

    [Header("Events")]
    [SerializeField]
    private UnityEvent m_LoseLifeEvent = null;
    [SerializeField]
    private UnityEvent m_EndGameEvent = null;
    #endregion

    #region Awake/Start/Update
    private void Start()
    {
        Vector3 l_Pos = m_CameraCenterPosition;
        l_Pos.z = m_CameraRaycastPlane.transform.position.z;
        m_CameraRaycastPlane.transform.position = l_Pos;
        Vector3 l_NewScale = Vector3.zero;
        l_NewScale.x = m_CameraSize.x / 10.0f;
        l_NewScale.y = m_CameraRaycastPlane.transform.localScale.y;
        l_NewScale.z = m_CameraSize.y / 10.0f;
        m_CameraRaycastPlane.transform.localScale = l_NewScale;

        StartCoroutine(SetupGame());
        StartCoroutine(BeginningTimer());
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
    private IEnumerator SetupGame()
    {
        m_PlayerCount = UsersManager.m_UsersInfos.Count;
        float l_TimeBetweenSpawns = m_TimeBeforeGameStart / m_PlayerCount;
        //Apparition et placement des joueurs
        foreach (UserInfos l_UserInfos in UsersManager.m_UsersInfos)
        {
            PlayerInfos l_SpawnedPLayer = SpawnPlayer(l_UserInfos);
            m_PlayersAlive.Add(l_SpawnedPLayer);
            m_Characters.Add(l_SpawnedPLayer.GetComponent<Health>());
            l_SpawnedPLayer.PlayerIndex = l_UserInfos.m_PlayerIndex;

            LinkHealthBar(l_UserInfos, l_SpawnedPLayer);
            yield return new WaitForSeconds(l_TimeBetweenSpawns);
        }

        SetupPlayersLayerAndCamera();
        m_PlayerCount = m_PlayersAlive.Count;
        StartGame();
    }
    private IEnumerator BeginningTimer()
    {
        float l_CurrentTimer = m_BeginningTimerSprites.Count;
        Color l_NewColor = m_BeginningTimer.color;
        while (l_CurrentTimer > 0.0f)
        {
            l_CurrentTimer = l_CurrentTimer - Time.deltaTime;
            m_BeginningTimer.sprite = m_BeginningTimerSprites[(int)l_CurrentTimer];
            l_NewColor.a = (l_CurrentTimer % 1);
            m_BeginningTimer.color = l_NewColor;
            yield return null;
        }
        m_BeginningTimer.gameObject.SetActive(false);
    }
    private PlayerInfos SpawnPlayer(UserInfos p_UserInfos)
    {
        PlayerInput l_SpawnedPlayer = PlayerInput.Instantiate(p_UserInfos.UserCharacter.CharacterPrefab, -1, null, -1, p_UserInfos.UserInputDevice);
        l_SpawnedPlayer.DeactivateInput();
        //Changer position du joueur
        l_SpawnedPlayer.GetComponent<CharacterController>().enabled = false;

        if (m_UsedSpawnPoints.Count < m_PlayersSpawn.Count)
        {
            while (true)
            {
                int l_SpawnPoint = Random.Range(0, m_PlayersSpawn.Count);
                if (!m_UsedSpawnPoints.Contains(l_SpawnPoint))
                {
                    l_SpawnedPlayer.transform.position = m_PlayersSpawn[l_SpawnPoint].position;
                    m_UsedSpawnPoints.Add(l_SpawnPoint);

                    break;
                }
                else
                {
                    continue;
                }
            }
        }
        else
        {
            int l_SpawnPoint = Random.Range(0, m_PlayersSpawn.Count);
            l_SpawnedPlayer.transform.position = m_PlayersSpawn[l_SpawnPoint].position;
        }

        l_SpawnedPlayer.GetComponent<CharacterController>().enabled = true;
        //Changer parent du joueur
        l_SpawnedPlayer.transform.SetParent(m_PlayersParent);
        //Setup les données du personnage
        l_SpawnedPlayer.GetComponent<CharacterInfos>().Character = p_UserInfos.UserCharacter;

        FindObjectOfType<SpawnerCamera>().SetWatchTarget(l_SpawnedPlayer.gameObject);

        return l_SpawnedPlayer.GetComponent<PlayerInfos>();
    }
    private void LinkHealthBar(UserInfos p_UserInfos, PlayerInfos p_PlayerInfos)
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
                m_HealthBars[i].m_PlayeIndex.text = "Player " + p_UserInfos.m_PlayerIndex;

                m_HealthBars[i].m_HealthBarLogo.gameObject.SetActive(true);
                l_HealthBar.gameObject.SetActive(true);

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
        PlayersCamera l_Camera = FindObjectOfType<PlayersCamera>(true);
        for (int i = 0; i < m_PlayersAlive.Count; i++)
        {
            m_PlayersAlive[i].AttackableLayers = m_PlayersLayers[i].AttackableLayer;
            m_PlayersAlive[i].gameObject.layer = m_PlayersLayers[i].PlayerLayer;
            foreach (Transform l_Child in m_PlayersAlive[i].gameObject.GetComponentsInChildren(typeof(Transform), true))
            {
                l_Child.gameObject.layer = m_PlayersLayers[i].PlayerLayer;
            }
            l_Camera.ListOfAllPlayers.Add(m_PlayersAlive[i].GetComponent<CharacterInfos>());
            m_PlayersAlive[i].GetComponent<PlayerInput>().ActivateInput();
        }

        l_Camera.SetGameManager(this);
        l_Camera.enabled = true;
        FindObjectOfType<SpawnerCamera>().enabled = false;
    }
    private void StartGame()
    {
        FindObjectOfType<SpawnerCamera>().gameObject.SetActive(false);
        PlayersCamera l_Camera = FindObjectOfType<PlayersCamera>(true);
        l_Camera.gameObject.SetActive(true);

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
            p_Character.DeathByEjection();
            m_LoseLifeEvent.Invoke();
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

            UsersManager.m_LoserCharacter.m_PlayedCharacter = p_Character.GetComponent<CharacterInfos>().Character;
            UsersManager.m_LoserCharacter.m_RemainingLives = p_Character.GetComponent<Health>().CurrentLives;

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

        Color l_NewColor = m_BeginningTimer.color;

        if (m_CurrentGameTimer <= 3.0f)
        {
            m_BeginningTimer.gameObject.SetActive(true);
            m_BeginningTimer.sprite = m_EndTimerSprites[(int)m_CurrentGameTimer];
            l_NewColor.a = (m_CurrentGameTimer % 1);
            m_BeginningTimer.color = l_NewColor;
        }
        if (m_CurrentGameTimer <= 0.0f)
        {
            //On vérifie les vies de tous les joueurs
            //Celui qui a le plus de vies a gagné
            Health l_LowestLives = null;
            Health l_HighestLives = null;
            for (int i = 0; i < m_Characters.Count; i++)
            {
                if (i == 0)
                {
                    l_HighestLives = m_Characters[i];
                    l_LowestLives = m_Characters[i];
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
                    }

                    if (l_LowestLives.CurrentLives > m_Characters[i].CurrentLives)
                    {
                        l_LowestLives = m_Characters[i];
                    }
                    else if(l_LowestLives.CurrentLives == m_Characters[i].CurrentLives)
                    {
                        if (l_LowestLives.CurrentHealth > m_Characters[i].CurrentHealth)
                        {
                            l_LowestLives = m_Characters[i];
                        }
                    }
                }
            }

            if (l_LowestLives == l_HighestLives)
            {
                EndGameDraw();
                return;
            }

            UsersManager.m_LoserCharacter.m_PlayedCharacter = l_LowestLives.GetComponent<CharacterInfos>().Character;
            UsersManager.m_LoserCharacter.m_RemainingLives = l_LowestLives.CurrentLives;
            UsersManager.m_LoserCharacter.m_PlayerIndex = l_LowestLives.GetComponent<PlayerInfos>().PlayerIndex;


            EndGame(l_HighestLives.GetComponent<PlayerInfos>());
        }
    }
    private void EndGame(PlayerInfos p_WinnerPlayerInfo)
    {
        m_GameState = EGameState.Ended;

        UsersManager.m_WinnerCharacter.m_PlayedCharacter = p_WinnerPlayerInfo.GetComponent<CharacterInfos>().Character;
        UsersManager.m_WinnerCharacter.m_RemainingLives = p_WinnerPlayerInfo.GetComponent<Health>().CurrentLives;
        UsersManager.m_WinnerCharacter.m_PlayerIndex = p_WinnerPlayerInfo.PlayerIndex;

        StartCoroutine(CheckForSceneChanging("VictoryScreen"));
        Time.timeScale = 0.2f;

        m_EndGameEvent.Invoke();
    }
    private void EndGameDraw()
    {
        m_GameState = EGameState.Ended;

        UsersManager.m_WinnerCharacter.m_PlayedCharacter = m_PlayersAlive[0].GetComponent<CharacterInfos>().Character;
        UsersManager.m_WinnerCharacter.m_RemainingLives = m_PlayersAlive[0].GetComponent<Health>().CurrentLives;
        UsersManager.m_WinnerCharacter.m_PlayerIndex = m_PlayersAlive[0].PlayerIndex;

        UsersManager.m_LoserCharacter.m_PlayedCharacter = m_PlayersAlive[1].GetComponent<CharacterInfos>().Character;
        UsersManager.m_LoserCharacter.m_RemainingLives = m_PlayersAlive[1].GetComponent<Health>().CurrentLives;
        UsersManager.m_LoserCharacter.m_PlayerIndex = m_PlayersAlive[1].PlayerIndex;

        StartCoroutine(CheckForSceneChanging("DrawScreen"));

        m_EndGameEvent.Invoke();
    }
    private IEnumerator CheckForSceneChanging(string p_SceneName)
    {
        m_VictorySceneAsync = SceneManager.LoadSceneAsync(p_SceneName);
        while (true)
        {
            if (m_VictorySceneAsync.isDone)
            {
                Time.timeScale = 1.0f;
                break;
            }
            yield return null;
        }
    }
    public IEnumerator RespawnTimer(GameObject p_Character)
    {
        p_Character.GetComponent<CharacterController>().enabled = false;
        p_Character.transform.position = m_PlayersSpawn[Random.Range(0, m_PlayersSpawn.Count)].position;

        yield return new WaitForSeconds(m_RespawnDelay);
        RespawnPlayer(p_Character);
    }
    private void RespawnPlayer(GameObject p_Character)
    {
        Component[] l_Components = p_Character.GetComponentsInChildren<Component>();
        foreach (Component l_Component in l_Components)
        {
            if (l_Component is CharacterController)
            {
                ((CharacterController)l_Component).enabled = true;
            }
            else if (l_Component is PlayerInput)
            {
                ((PlayerInput)l_Component).ActivateInput();
            }
            else if (l_Component is MonoBehaviour)
            {
                ((MonoBehaviour)l_Component).enabled = true;
            }
            else if (l_Component is SpriteRenderer)
            {
                ((SpriteRenderer)l_Component).enabled = true;
            }
        }
    }
    #endregion


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(m_MapCenterPosition.x, m_MapCenterPosition.y, m_MapCenterPosition.z), new Vector3(m_MapSize.x, m_MapSize.y, 0));
        Gizmos.DrawWireCube(new Vector3(m_CameraCenterPosition.x, m_CameraCenterPosition.y, m_CameraCenterPosition.z), new Vector3(m_CameraSize.x, m_CameraSize.y, 0));
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
        public Text m_PlayeIndex = null;
    }
}


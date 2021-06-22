using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersCamera : MonoBehaviour, IUpdateUser
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
    [SerializeField]
    private List<CharacterInfos> m_ListOfAllPlayers = new List<CharacterInfos>();
    public List<CharacterInfos> ListOfAllPlayers { get { return m_ListOfAllPlayers; } }
    [SerializeField]
    private GameObject m_MainCamera = null;
    private GameManager m_GameManager = null;

    private Vector3 m_AveragePositionPlayers = Vector3.zero;

    [SerializeField]
    private AnimationCurve m_CameraZoomCurve = null;

    private float m_GreaterDistancePlayers = 0;

    private Vector3 m_BaseCameraPosition = Vector3.zero;
    private Vector3 m_CameraTargetPosition = Vector3.zero;

    float m_XLeft = 0.0f;
    float m_XRight = 0.0f;
    float m_YUp = 0.0f;
    float m_YDown = 0.0f;

    private Vector3 m_LastPosition = Vector3.zero;
    [SerializeField]
    private LayerMask m_LayerMask = 0;
    [SerializeField]
    private LayerMask m_LayerMaskOut = 0;
    #endregion
    #region Awake/Start/Update
    private void Start()
    {
        m_BaseCameraPosition = m_MainCamera.transform.position;

        m_XLeft = m_GameManager.CameraCenterPosition.x - m_GameManager.CameraSize.x / 2 + 0.5f;
        m_XRight = m_GameManager.CameraCenterPosition.x + m_GameManager.CameraSize.x / 2 - 0.5f;
        m_YUp = m_GameManager.CameraCenterPosition.y + m_GameManager.CameraSize.y / 2 - 0.5f;
        m_YDown = m_GameManager.CameraCenterPosition.y - m_GameManager.CameraSize.y / 2 + 0.5f;
    }
    public void CustomUpdate(float p_DeltaTime)
    {
        //if (m_ListOfAllPlayers.Count != 0)
        //{
        //    CameraPositioning(p_DeltaTime);
        //}
    }
    private void LateUpdate()
    {
        if (m_ListOfAllPlayers.Count != 0)
        {
            CameraPositioning(Time.deltaTime);
        }
    }
    #endregion
    #region Functions
    public void SetGameManager(GameManager p_GameManager)
    {
        m_GameManager = p_GameManager;
    }
    private void CameraPositioning(float p_DeltaTime)
    {
        for (int i = 0; i < m_ListOfAllPlayers.Count; i++)
        {
            m_AveragePositionPlayers += m_ListOfAllPlayers[i].transform.position;
        }
        m_AveragePositionPlayers /= m_ListOfAllPlayers.Count;
        m_AveragePositionPlayers = new Vector3(m_AveragePositionPlayers.x, m_AveragePositionPlayers.y, 0);
        m_GreaterDistancePlayers = 0;
        for (int i = 0; i < m_ListOfAllPlayers.Count; i++)
        {
            for (int v = 0; v < m_ListOfAllPlayers.Count; v++)
            {
                if (m_GreaterDistancePlayers < Mathf.Sqrt(Mathf.Pow(m_ListOfAllPlayers[v].transform.position.x - m_ListOfAllPlayers[i].transform.position.x, 2) + Mathf.Pow((m_ListOfAllPlayers[v].transform.position.y - m_ListOfAllPlayers[i].transform.position.y) * 2.5f, 2))) /*Vector3.Distance(m_ListOfAllPlayers[i].transform.position, m_ListOfAllPlayers[v].transform.position)*/
                {
                    m_GreaterDistancePlayers = Mathf.Sqrt(Mathf.Pow(m_ListOfAllPlayers[v].transform.position.x - m_ListOfAllPlayers[i].transform.position.x, 2) + Mathf.Pow((m_ListOfAllPlayers[v].transform.position.y - m_ListOfAllPlayers[i].transform.position.y) * 2.5f, 2));
                }
            }
        }

        m_CameraTargetPosition = m_AveragePositionPlayers + new Vector3(0, 0, m_BaseCameraPosition.z + m_CameraZoomCurve.Evaluate(m_GreaterDistancePlayers / 2));
        m_LastPosition = m_MainCamera.transform.position;

        //Rajouter ici la modification de la m_TargetPosition
        m_CameraTargetPosition = CheckForNewPosition(p_DeltaTime);
        //Jusqu'ici

        //Passer la caméra à la nouvelle position
        m_MainCamera.transform.position = Vector3.Lerp(m_LastPosition, m_CameraTargetPosition, 0.1f);
        //m_MainCamera.transform.position = m_CameraTargetPosition;

        //Reset des valeurs
        m_AveragePositionPlayers = Vector3.zero;
    }
    private Vector3 CheckForNewPosition(float p_DeltaTime)
    {
        float l_XOffset = 0.0f;
        float l_YOffset = 0.0f;

        //m_MainCamera.transform.position = Vector3.Lerp(m_LastPosition, m_CameraTargetPosition, 0.1f);
        m_MainCamera.transform.position = m_CameraTargetPosition;
        Camera l_Camera = m_MainCamera.GetComponent<Camera>();
        //Raycast gauche
        //Debug.DrawLine(l_Camera.transform.position, l_Camera.transform.position + l_Camera.ScreenPointToRay(new Vector3(0, l_Camera.pixelHeight / 2)).direction, Color.yellow, 10f);
        //Debug.DrawRay(l_Camera.ScreenPointToRay(new Vector3(0, l_Camera.pixelHeight / 2)).origin, l_Camera.ScreenPointToRay(new Vector3(0, l_Camera.pixelHeight / 2)).direction * 1000, Color.red, .1f);
        if (!Physics.Raycast(l_Camera.ScreenPointToRay(new Vector3(0, l_Camera.pixelHeight / 2)), 1000.0f, m_LayerMask))
        {
            if (Physics.Raycast(l_Camera.ScreenPointToRay(new Vector3(0, l_Camera.pixelHeight / 2)), out RaycastHit l_HitInfo, 1000.0f, m_LayerMaskOut))
            {
                l_XOffset = m_XLeft - l_HitInfo.point.x;
            }
        }
        //Raycast bas
        //Debug.DrawRay(l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth / 2, 0)).origin, l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth / 2, 0)).direction * 1000, Color.red, .1f);
        if (!Physics.Raycast(l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth / 2, 0)), 1000.0f, m_LayerMask))
        {
            if (Physics.Raycast(l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth / 2, 0)), out RaycastHit l_HitInfo, 1000.0f, m_LayerMaskOut))
            {
                if (l_HitInfo.point.y <= m_YDown)
                {
                    l_YOffset = m_YDown - l_HitInfo.point.y;
                }
            }
        }
        //Raycast haut
        //Debug.DrawRay(l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth / 2, l_Camera.pixelHeight - 1)).origin, l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth / 2, l_Camera.pixelHeight - 1)).direction * 1000, Color.red, .1f);
        if (!Physics.Raycast(l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth / 2, l_Camera.pixelHeight - 1)), 1000.0f, m_LayerMask))
        {
            if (Physics.Raycast(l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth / 2, l_Camera.pixelHeight - 1)), out RaycastHit l_HitInfo, 1000.0f, m_LayerMaskOut))
            {
                if (l_HitInfo.point.y >= m_YUp)
                {
                    l_YOffset = m_YUp - l_HitInfo.point.y;
                }
            }
        }
        //Raycast droite
        //Debug.DrawRay(l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth - 1, l_Camera.pixelHeight / 2)).origin, l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth - 1, l_Camera.pixelHeight / 2)).direction * 1000, Color.red, .1f);
        if (!Physics.Raycast(l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth - 1, l_Camera.pixelHeight / 2)), 1000.0f, m_LayerMask))
        {
            if (Physics.Raycast(l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth - 1, l_Camera.pixelHeight / 2)), out RaycastHit l_HitInfo, 1000.0f, m_LayerMaskOut))
            {
                l_XOffset = m_XRight - l_HitInfo.point.x;
            }
        }

        m_CameraTargetPosition.x = m_CameraTargetPosition.x + l_XOffset;
        m_CameraTargetPosition.y = m_CameraTargetPosition.y + l_YOffset;
        return m_CameraTargetPosition;
    }
    #endregion
}
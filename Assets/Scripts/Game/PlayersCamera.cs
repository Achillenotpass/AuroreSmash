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

    private Vector3 m_LastPosition = Vector3.zero;
    [SerializeField]
    private LayerMask m_LayerMask = 0;
    [SerializeField]
    private LayerMask m_LayerMaskOut = 0;
    #endregion

    private void Start()
    {
        m_BaseCameraPosition = m_MainCamera.transform.position;
    }

    public void CustomUpdate(float p_DeltaTime)
    {
        if (m_ListOfAllPlayers.Count != 0)
        {
            CameraPositioning(p_DeltaTime);
        }
    }

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
        m_MainCamera.transform.position = Vector3.Lerp(m_MainCamera.transform.position, m_CameraTargetPosition, 0.1f);

        //CheckForNewPosition(p_DeltaTime);
        //m_MainCamera.transform.position = Vector3.Lerp(m_LastPosition, m_CameraTargetPosition, 0.1f); ;


        m_AveragePositionPlayers = Vector3.zero;
    }
    
    private void CheckForNewPosition(float p_DeltaTime)
    {
        float l_XOffset = 0.0f;
        float l_YOffset = 0.0f;

        Camera l_Camera = m_MainCamera.GetComponent<Camera>();
        //Raycast gauche
        if (!Physics.Raycast(l_Camera.ScreenPointToRay(new Vector3(0, l_Camera.pixelHeight / 2)), 1000.0f, m_LayerMask))
        {
            if (Physics.Raycast(l_Camera.ScreenPointToRay(new Vector3(0, l_Camera.pixelHeight / 2)), out RaycastHit l_HitInfo, 1000.0f, m_LayerMaskOut))
            {
                Debug.Log("point x" + l_HitInfo.point.x);
                //Center       -  ( Camera center      -        size)
                l_XOffset = l_HitInfo.point.x - (m_GameManager.CameraCenterPosition.x - m_GameManager.CameraSize.x);
            }
        }
        //Raycast haut
        if (!Physics.Raycast(l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth / 2, 0)), 1000.0f, m_LayerMask))
        {
            if (Physics.Raycast(l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth / 2, 0)), out RaycastHit l_HitInfo, 1000.0f, m_LayerMaskOut))
            {
                //Center       -  ( Camera center      +        size)
                l_YOffset = l_HitInfo.point.y - (m_GameManager.CameraCenterPosition.y + m_GameManager.CameraSize.y);
            }
        }
        //Raycast bas
        if (!Physics.Raycast(l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth / 2, l_Camera.pixelHeight - 1)), 1000.0f, m_LayerMask))
        {
            if (Physics.Raycast(l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth / 2, l_Camera.pixelHeight - 1)), out RaycastHit l_HitInfo, 1000.0f, m_LayerMaskOut))
            {
                //Center       -  ( Camera center      -        size)
                l_YOffset = l_HitInfo.point.y - (m_GameManager.CameraCenterPosition.y - m_GameManager.CameraSize.y);
            }
        }
        //Raycast droite
        if (!Physics.Raycast(l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth - 1, l_Camera.pixelHeight / 2)), 1000.0f, m_LayerMask))
        {
            if (Physics.Raycast(l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth - 1, l_Camera.pixelHeight / 2)), out RaycastHit l_HitInfo, 1000.0f, m_LayerMaskOut))
            {
                //Center       -  ( Camera center      +        size)
                l_XOffset = l_HitInfo.point.x - (m_GameManager.CameraCenterPosition.x + m_GameManager.CameraSize.x);
            }
        }
        Debug.Log("x : " + l_XOffset);
        Debug.Log("y : " + l_YOffset);
        Vector3 l_Pos = m_MainCamera.transform.position;
        l_Pos.x = m_MainCamera.transform.position.x + l_XOffset;
        l_Pos.y = m_MainCamera.transform.position.y + l_YOffset;

        m_MainCamera.transform.position = l_Pos;
        //m_CameraTargetPosition.x = m_CameraTargetPosition.x + l_XOffset;
        //m_CameraTargetPosition.y = m_CameraTargetPosition.y + l_YOffset;
    }
}
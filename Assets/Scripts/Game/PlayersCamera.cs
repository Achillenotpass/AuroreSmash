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
    [SerializeField]
    private List<CharacterInfos> m_ListOfAllPlayers = new List<CharacterInfos>();
    public List<CharacterInfos> ListOfAllPlayers { get { return m_ListOfAllPlayers; } }
    [SerializeField]
    private GameObject m_MainCamera = null;

    private Vector3 m_AveragePositionPlayers = Vector3.zero;

    [SerializeField]
    private AnimationCurve m_CameraZoomCurve = null;

    private float m_GreaterDistancePlayers = 0;

    private Vector3 m_BaseCameraPosition = Vector3.zero;
    private Vector3 m_CameraTargetPosition = Vector3.zero;

    private Vector3 m_LastPosition = Vector3.zero;

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

        m_AveragePositionPlayers = Vector3.zero;
    }
    
    private void CheckForNewPosition()
    {
        Camera l_Camera = m_MainCamera.GetComponent<Camera>();
        int l_LayerMask = 1 << 14;
        Vector3 l_NewPosition = m_MainCamera.transform.position;
        //Raycast gauche
        if (!Physics.Raycast(l_Camera.ScreenPointToRay(new Vector3(0, l_Camera.pixelHeight / 2)), 1000.0f, l_LayerMask))
        {
            l_NewPosition.x = m_LastPosition.x;
        }
        //Raycast haut
        if (!Physics.Raycast(l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth / 2, l_Camera.pixelHeight - 1)), 1000.0f, l_LayerMask))
        {
            l_NewPosition.y = m_LastPosition.y;
        }
        //Raycast bas
        if (!Physics.Raycast(l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth / 2, 0)), 1000.0f, l_LayerMask))
        {
            l_NewPosition.y = m_LastPosition.y;
        }
        //Raycast droite
        if (!Physics.Raycast(l_Camera.ScreenPointToRay(new Vector3(l_Camera.pixelWidth - 1, l_Camera.pixelHeight / 2)), 1000.0f, l_LayerMask))
        {
            l_NewPosition.x = m_LastPosition.x;
        }

        m_MainCamera.transform.position = l_NewPosition;
    }
    private void CameraShake(float p_ShakePower)
    {

    }
}
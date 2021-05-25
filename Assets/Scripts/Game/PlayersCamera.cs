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

    private List<CharacterInfos> m_ListOfAllPlayers = new List<CharacterInfos>();
    public List<CharacterInfos> ListOfAllPlayers { get { return m_ListOfAllPlayers; } }

    private GameObject m_MainCamera = null;

    private Vector3 m_AveragePositionPlayers = Vector3.zero;

    [SerializeField]
    private AnimationCurve m_CameraZoomCurve = null;

    [SerializeField]
    private float m_CameraDefaultZoom = -10;

    private float m_GreaterDistancePlayers = 0;

    private Vector3 m_BaseCameraPosition = Vector3.zero;

    [SerializeField]
    private GameObject m_LevelCenter = null;

    private void Start()
    {
        m_MainCamera = this.gameObject;
        m_BaseCameraPosition = m_MainCamera.transform.position;
        CharacterInfos[] l_ListOfAllPlayers = FindObjectsOfType<CharacterInfos>();
        for(int i = 0; i < l_ListOfAllPlayers.Length; i++)
        {
            m_ListOfAllPlayers.Add(l_ListOfAllPlayers[i]);
        }
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
        m_MainCamera.transform.position = m_AveragePositionPlayers + new Vector3(0, 0, m_BaseCameraPosition.z + m_CameraZoomCurve.Evaluate(m_GreaterDistancePlayers / 2));
        m_AveragePositionPlayers = Vector3.zero;
    }

    private void CameraShake(float p_ShakePower)
    {

    }
}
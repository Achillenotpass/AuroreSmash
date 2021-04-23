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

    private CharacterInfos[] m_ListOfAllPlayers;

    private GameObject m_MainCamera = null;

    private Vector3 m_AveragePositionPlayers = Vector3.zero;

    [SerializeField]
    private AnimationCurve m_CameraZoomCurve = null;

    [SerializeField]
    private float m_CameraDefaultZoom = -10;

    private float m_GreaterDistancePlayers = 0;

    private void Start()
    {
        m_ListOfAllPlayers = FindObjectsOfType<CharacterInfos>();
        m_MainCamera = this.gameObject;
    }

    public void CustomUpdate(float p_DeltaTime)
    {
        for(int i = 0; i < m_ListOfAllPlayers.Length; i++)
        {
            m_AveragePositionPlayers += m_ListOfAllPlayers[i].transform.position;
        }
        m_AveragePositionPlayers /= m_ListOfAllPlayers.Length;
        m_GreaterDistancePlayers = 0;
        for (int i = 0; i < m_ListOfAllPlayers.Length; i++)
        {
            for (int v = 0; v < m_ListOfAllPlayers.Length; v++)
            {
                if(m_GreaterDistancePlayers < Vector3.Distance(m_ListOfAllPlayers[i].transform.position, m_ListOfAllPlayers[v].transform.position))
                {
                    m_GreaterDistancePlayers = Vector3.Distance(m_ListOfAllPlayers[i].transform.position, m_ListOfAllPlayers[v].transform.position);
                }
            }
        }
        Debug.Log(m_GreaterDistancePlayers);
        m_MainCamera.transform.position = m_AveragePositionPlayers + new Vector3(0, 0, m_CameraZoomCurve.Evaluate(m_GreaterDistancePlayers/2));
        
        
    }
}
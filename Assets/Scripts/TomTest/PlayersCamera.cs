using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersCamera : MonoBehaviour, IUpdateUser
{
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

    private PlayerInfos[] m_ListOfAllPlayers;

    private void Start()
    {
        m_ListOfAllPlayers = FindObjectsOfType<PlayerInfos>();
    }

    public void CustomUpdate(float p_DeltaTime)
    {

    }
}

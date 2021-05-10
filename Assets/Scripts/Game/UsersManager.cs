using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class UsersManager : MonoBehaviour
{
    //This script as it is currently (last updated on 10/05/2021) allows for players to spawn on play
    [SerializeField]
    private Transform m_CharactersParent = null;
    [SerializeField]
    private List<SO_Character> m_Characters;
    [SerializeField]
    private List<InputDevice> m_Devices = new List<InputDevice>();
    public List<InputDevice> Devices { get { return m_Devices; } }
    [SerializeField]
    private List<Transform> m_SpawnPoints = new List<Transform>();

    private void Awake()
    {
        m_Devices.Add(InputSystem.devices[0]);
        m_Devices.Add(InputSystem.devices[1]);
        SpawnCharacters();
    }

    [ContextMenu("Spawn players")]
    public void SpawnCharacters()
    {
        for (int i = 0; i < m_Devices.Count; i++)
        {
            PlayerInput l_Object = PlayerInput.Instantiate(m_Characters[i].CharacterPrefab, -1, null, -1, m_Devices[i]);
            l_Object.transform.SetParent(m_CharactersParent);
            l_Object.transform.position = m_SpawnPoints[i].position;
        }
    }
}

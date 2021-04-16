using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInfos : MonoBehaviour
{
    [SerializeField]
    private LayerMask m_AttackableLayers;
    public LayerMask AttackableLayers { get { return m_AttackableLayers; } }
    [SerializeField]
    private int m_DeviceID = 1;
    public int DeviceID { get { return m_DeviceID; } }
    static public int s_CurrentGamepad = 0;

    
    private void Start()
    {
        var l_AllGamepads = Gamepad.all;
        m_DeviceID = l_AllGamepads[s_CurrentGamepad].deviceId;
        s_CurrentGamepad = s_CurrentGamepad + 1;
    }
}

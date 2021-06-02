using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerCamera : MonoBehaviour
{
    private Vector3 m_Target = Vector3.zero;
    [SerializeField]
    private float m_ZoomLevel = 10.0f;

    private void Awake()
    {
        m_Target = transform.position;
    }
    private void Update()
    {
        WatchTarget(m_Target);
    }
    public void SetWatchTarget(GameObject p_Character)
    {
        m_Target = p_Character.transform.position;
    }
    public void WatchTarget(Vector3 p_TargetPosition)
    {
        Vector3 l_NewPosition = Vector3.zero;

        l_NewPosition.x = Mathf.Lerp(transform.position.x, p_TargetPosition.x, 0.1f);
        l_NewPosition.y = Mathf.Lerp(transform.position.y, p_TargetPosition.y, 0.1f);
        l_NewPosition.z = Mathf.Lerp(transform.position.z, p_TargetPosition.z - m_ZoomLevel, 0.1f);

        transform.position = l_NewPosition;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTimeScale : MonoBehaviour
{
    [SerializeField, Range(0.0f, 1.0f)]
    private float m_TimeScale = 1;

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = m_TimeScale;
    }
}

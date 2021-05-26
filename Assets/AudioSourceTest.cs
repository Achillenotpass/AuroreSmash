using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioSourceTest : MonoBehaviour
{
    [SerializeField]
    private UnityEvent m_AudioSourceEvent = new UnityEvent();
    [ContextMenu("Audio test")]
    public void CallEventAudioSource()
    {
        m_AudioSourceEvent.Invoke();
    }
}

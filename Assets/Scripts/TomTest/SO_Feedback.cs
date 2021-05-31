using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFeedback", menuName = "ScriptableObjects/Gameplay/Feedbacks/Feedback")]
public class SO_Feedback : ScriptableObject
{
    [SerializeField]
    private string m_FeedbackName = null;
    public string FeedbackName { get { return m_FeedbackName; } }

    [SerializeField]
    private ParticleSystem[] m_ParticleSystemList = null;
    public ParticleSystem[] ParticleSystemList { get { return m_ParticleSystemList; } }

    [SerializeField]
    private AudioClip[] m_AudioClipList = null;
    public AudioClip[] AudioClipList { get { return m_AudioClipList; } }
}
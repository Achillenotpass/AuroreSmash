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
    private bool m_AsParticle = false;
    public bool AsParticle { get { return m_AsParticle; } }
    [SerializeField]
    private ParticleSystem[] m_ParticleSystemList = null;
    public ParticleSystem[] ParticleSystemList { get { return m_ParticleSystemList; } }

    [SerializeField]
    private bool m_AsAudioClip = false;
    public bool AsAudioClip { get { return m_AsAudioClip; } }
    [SerializeField]
    private AudioClip[] m_AudioClipList = null;
    public AudioClip[] AudioClipList { get { return m_AudioClipList; } }

    [SerializeField]
    private bool m_AsToShake = false;
    public bool AsToShake { get { return m_AsToShake; } }
    [SerializeField]
    private float m_ShakeIntesity = 0.1f;
    public float ShakeIntesity { get { return m_ShakeIntesity; } }
    [SerializeField]
    private float m_ShakeDuration = 0.1f;
    public float ShakeDuration { get { return m_ShakeDuration; } }

    [SerializeField]
    private bool m_AsToBlink = false;
    public bool AsToBlink { get { return m_AsToBlink; } }
    [SerializeField]
    private float m_BlinkIntensity = 0.1f;
    public float BlinkIntensity { get { return m_BlinkIntensity; } }
    [SerializeField]
    private float m_BlinkDuration = 0.1f;
    public float BlinkDuration { get { return m_BlinkDuration; } }

    [SerializeField]
    private bool m_AsToFreezeFrame = false;
    public bool AsToFreezeFrame { get { return m_AsToFreezeFrame; } }
    [SerializeField]
    private float m_FreezeFrameDuration = 0.1f;
    public float FreezeFrameDuration { get { return m_FreezeFrameDuration; } }

    [SerializeField]
    private bool m_AsToVibrate = false;
    public bool AsToVibrate { get { return m_AsToVibrate; } }
    [SerializeField]
    private float m_VibrationIntesity = 0.1f;
    public float VibrationIntesity { get { return m_VibrationIntesity; } }
    [SerializeField]
    private float m_VibrationDuration = 0.1f;
    public float VibrationDuration { get { return m_VibrationDuration; } }


}
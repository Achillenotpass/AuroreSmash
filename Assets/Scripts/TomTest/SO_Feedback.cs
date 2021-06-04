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
    private bool m_AsVFX = false;
    [SerializeField]
    private GameObject[] m_VFXList = null;
    public GameObject[] VFXList { get { return m_VFXList; } }
    [SerializeField]
    private Vector3[] m_VFXSpecificPosition = null;
    [SerializeField]
    private bool[] m_VFXAsToBeChild = null;

    public void InstantiateVFX(GameObject p_Instantiator, int p_NumberInTheList)
    {
        if (m_AsVFX)
        {
            Vector3 l_VFXSpecificPosition = m_VFXSpecificPosition[p_NumberInTheList];
            if (p_Instantiator.transform.localScale.x < 0)
                l_VFXSpecificPosition.x *= -1;
            GameObject l_VFXInstiated = Instantiate(m_VFXList[p_NumberInTheList], l_VFXSpecificPosition, Quaternion.identity);
            if (m_VFXAsToBeChild[p_NumberInTheList])
                l_VFXInstiated.transform.parent = p_Instantiator.transform;
        }
    }
    public void InstantiateAllVFX(GameObject p_Instantiator)
    {
        if (m_AsVFX)
        {
            for (int i = 0; i < m_VFXList.Length; i++)
            {
                Vector3 l_VFXSpecificPosition = m_VFXSpecificPosition[i];
                if (p_Instantiator.transform.localScale.x < 0)
                    l_VFXSpecificPosition.x *= -1;
                GameObject l_VFXInstiated = Instantiate(m_VFXList[i], p_Instantiator.transform.position + l_VFXSpecificPosition, Quaternion.identity);
                if (m_VFXAsToBeChild[i])
                    l_VFXInstiated.transform.parent = p_Instantiator.transform;
            }
        }
    }

    [SerializeField]
    private bool m_AsSFX = false;
    [SerializeField]
    private AudioClip[] m_SFXList = null;
    public AudioClip[] SFXList { get { return m_SFXList; } }

    public void InstantiateAudioClip(int p_NumberInTheList)
    {
        if(m_AsSFX)
            FindObjectOfType<AudioSource>().PlayOneShot(m_SFXList[p_NumberInTheList]);
    }
    public void InstantiateAllAudioClip()
    {
        if(m_AsSFX)
            foreach (AudioClip l_AudioClip in m_SFXList)
                FindObjectOfType<AudioSource>().PlayOneShot(l_AudioClip);
    }

    [SerializeField]
    private bool m_AsVFXAnimation = false;
    [SerializeField]
    private GameObject[] m_VFXAnimation = null;
    [SerializeField]
    private Vector3[] m_VFXAnimationSpecificPosition = null;
    [SerializeField]
    private bool[] m_VFXAnimationAsToBeChild = null;
    
    
    public void InstantiateVFXAnimation(GameObject p_Instantiator, int p_NumberInTheList)
    {
        if (m_AsVFXAnimation)
        {
            Vector3 l_VFXAnimationSpecificPosition = m_VFXAnimationSpecificPosition[p_NumberInTheList];
            if (p_Instantiator.transform.localScale.x < 0)
                l_VFXAnimationSpecificPosition.x *= -1;

            GameObject l_VFXAnimation = Instantiate(m_VFXAnimation[p_NumberInTheList], l_VFXAnimationSpecificPosition, Quaternion.identity);
            if (m_VFXAnimationAsToBeChild[p_NumberInTheList])
                l_VFXAnimation.transform.parent = p_Instantiator.transform;
        }
    }

    public void InstantiateAllVFXAnimation(GameObject p_Instantiator)
    {
        if (m_AsVFXAnimation)
        {
            for(int i = 0; i < m_VFXAnimation.Length; i++)
            {
                Vector3 l_VFXAnimationSpecificPosition = m_VFXAnimationSpecificPosition[i];
                if (p_Instantiator.transform.localScale.x < 0)
                    l_VFXAnimationSpecificPosition.x *= -1;

                GameObject l_VFXAnimation = Instantiate(m_VFXAnimation[i], p_Instantiator.transform.position + l_VFXAnimationSpecificPosition, Quaternion.identity);
                if (m_VFXAnimationAsToBeChild[i])
                    l_VFXAnimation.transform.parent = p_Instantiator.transform;
            }
        }
    }

    [SerializeField]
    private bool m_AsToShake = false;
    public bool AsToShake { get { return m_AsToShake; } }
    [SerializeField]
    private float m_ShakeIntesity = 0.1f;
    public float ShakeIntesity { get { return m_ShakeIntesity; } }
    [SerializeField]
    private float m_ShakeDuration = 0.1f;
    public float ShakeDuration { get { return m_ShakeDuration; } }

    public void Shaking(Shake p_Shake, float p_ShakeIntensity, float p_ShakeDuration, GameObject p_ObjectToShake)
    {
        if(m_AsToShake)
            p_Shake.StartCoroutine(FindObjectOfType<Shake>().CreateShake(p_ShakeIntensity, p_ShakeDuration, p_ObjectToShake));
    }

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


    public void PlayFeedback(GameObject p_Initiator)
    {
        InstantiateAllVFX(p_Initiator);
        InstantiateAllVFXAnimation(p_Initiator);
        InstantiateAllAudioClip();
    }
}
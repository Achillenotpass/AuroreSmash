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
    private GameObject[] m_VFXList = null;
    public GameObject[] VFXList { get { return m_VFXList; } }

    public void InstantiateParticule(GameObject p_Instantiator, Transform p_SpecificPosition, int p_NumberInTheList)
    {
        GameObject l_VFXInstiated = Instantiate(m_VFXList[p_NumberInTheList]);
        l_VFXInstiated.transform.localPosition = p_SpecificPosition.position;
        if (p_Instantiator != null)
            l_VFXInstiated.transform.parent = p_Instantiator.transform;
    }
    public void InstantiateAllParticle(GameObject p_Instantiator, Vector3 p_SpecificPosition)
    {
        foreach (GameObject l_Particle in m_VFXList)
        {
            GameObject l_VFXInstiated = Instantiate(l_Particle);
            l_VFXInstiated.transform.localPosition = p_SpecificPosition;
            if (p_Instantiator != null)
                l_VFXInstiated.transform.parent = p_Instantiator.transform;
        }
    }


    [SerializeField]
    private AudioClip[] m_SFXList = null;
    public AudioClip[] SFXList { get { return m_SFXList; } }

    public void InstantiateAudioClip(int p_NumberInTheList)
    {
        FindObjectOfType<AudioSource>().PlayOneShot(m_SFXList[p_NumberInTheList]);
    }
    public void InstantiateAllAudioClip()
    {
        foreach (AudioClip l_AudioClip in m_SFXList)
        {
            FindObjectOfType<AudioSource>().PlayOneShot(l_AudioClip);
        }
    }

    [SerializeField]
    private SO_Animation[] m_VFXAnimationList = null;
    public SO_Animation[] VFXAnimationList { get { return m_VFXAnimationList; } }
    [SerializeField]
    private GameObject m_VFXAnimationInstantiator = null;
    
    public void InstantiateVFXAnimation(GameObject p_Instantiator, Vector3 p_SpecificPosition, int p_NumberInTheList)
    {
        GameObject l_VFXAnimationInstantiator = Instantiate(m_VFXAnimationInstantiator);
        if (p_Instantiator != null)
            l_VFXAnimationInstantiator.transform.parent = p_Instantiator.transform;
        l_VFXAnimationInstantiator.transform.position = p_SpecificPosition;
        List<Animation> l_Animations = new List<Animation>();
        l_Animations[0].m_AnimationName = VFXAnimationList[p_NumberInTheList].name;
        l_Animations[0].m_Animation = VFXAnimationList[p_NumberInTheList];
        l_VFXAnimationInstantiator.GetComponent<FramePerfectAnimator>().Animations = l_Animations;
    }

    public void InstantiateVFXAnimation(GameObject p_Instantiator, Vector3 p_SpecificPosition)
    {
        foreach (SO_Animation l_VFXAnimationList in m_VFXAnimationList)
        {
            GameObject l_VFXAnimationInstantiator = Instantiate(m_VFXAnimationInstantiator);
            if (p_Instantiator != null)
                l_VFXAnimationInstantiator.transform.parent = p_Instantiator.transform;
            l_VFXAnimationInstantiator.transform.position = p_SpecificPosition;
            List<Animation> l_Animations = new List<Animation>();
            l_Animations[0].m_AnimationName = l_VFXAnimationList.name;
            l_Animations[0].m_Animation = l_VFXAnimationList;
            l_VFXAnimationInstantiator.GetComponent<FramePerfectAnimator>().Animations = l_Animations;
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
}
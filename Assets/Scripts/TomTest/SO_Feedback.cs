using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFeedback", menuName = "ScriptableObjects/Gameplay/Feedbacks/Feedback")]
public class SO_Feedback : ScriptableObject
{
    [SerializeField]
    private string m_FeedbackName = null;
    public string FeedbackName { get { return m_FeedbackName; } }


    [Header("VFX")]
    [SerializeField]
    private bool m_HasVFX = false;
    [SerializeField]
    private GameObject[] m_VFXList = null;
    public GameObject[] VFXList { get { return m_VFXList; } }
    [SerializeField]
    private Vector3[] m_VFXSpecificPosition = null;
    [SerializeField]
    private Vector3[] m_VFXSpecificRotation = null;
    [SerializeField]
    private bool[] m_VFXHasToBeChild = null;
    [SerializeField]
    private bool[] m_VFXHasToBeChildOfChild = null;
    [SerializeField]
    private int[] m_VfxChildOfChild = null;
    [SerializeField]
    private int[] m_VFXChildOfChildNumber = null;
    [SerializeField]
    private bool m_VFXIsAlone = false;
    [SerializeField]
    private bool m_VFXHasToDisappearSmoothly = false;

    public void InstantiateVFX(GameObject p_Instantiator, int p_NumberInTheList)
    {
        if (m_HasVFX)
        {
            if (!m_VFXHasToBeChild[p_NumberInTheList] && !m_VFXHasToBeChildOfChild[p_NumberInTheList])
            {
                if (!m_VFXIsAlone || m_VFXIsAlone && FindObjectOfType<ParticleSystem>().gameObject.name != m_VFXList[p_NumberInTheList].name)
                {
                    Vector3 l_VFXSpecificPosition = m_VFXSpecificPosition[p_NumberInTheList];
                    if (l_VFXSpecificPosition.x / Mathf.Abs(l_VFXSpecificPosition.x) != p_Instantiator.transform.localScale.x / Mathf.Abs(p_Instantiator.transform.localScale.x))
                    {
                        l_VFXSpecificPosition.x *= -1;
                    }
                    GameObject l_VFXInstiated = Instantiate(m_VFXList[p_NumberInTheList], p_Instantiator.transform.position + l_VFXSpecificPosition, Quaternion.Euler(Quaternion.identity.x + m_VFXSpecificRotation[p_NumberInTheList].x, Quaternion.identity.y + m_VFXSpecificRotation[p_NumberInTheList].y, Quaternion.identity.z + m_VFXSpecificRotation[p_NumberInTheList].z));
                    l_VFXInstiated.name = m_VFXList[p_NumberInTheList].name;
                }
            }
            else if (m_VFXHasToBeChild[p_NumberInTheList])
            {
                if (!m_VFXIsAlone || m_VFXIsAlone && !p_Instantiator.transform.Find(m_VFXList[p_NumberInTheList].name))
                {
                    Vector3 l_VFXSpecificPosition = m_VFXSpecificPosition[p_NumberInTheList];
                    if (l_VFXSpecificPosition.x / Mathf.Abs(l_VFXSpecificPosition.x) != p_Instantiator.transform.localScale.x / Mathf.Abs(p_Instantiator.transform.localScale.x))
                    {
                        l_VFXSpecificPosition.x *= -1;
                    }
                    GameObject l_VFXInstiated = Instantiate(m_VFXList[p_NumberInTheList], p_Instantiator.transform.position + l_VFXSpecificPosition, Quaternion.Euler(Quaternion.identity.x + m_VFXSpecificRotation[p_NumberInTheList].x, Quaternion.identity.y + m_VFXSpecificRotation[p_NumberInTheList].y, Quaternion.identity.z + m_VFXSpecificRotation[p_NumberInTheList].z));
                    l_VFXInstiated.name = m_VFXList[p_NumberInTheList].name;
                    l_VFXInstiated.transform.parent = p_Instantiator.transform;
                }
            }
            else if (m_VFXHasToBeChildOfChild[p_NumberInTheList])
            {
                if (!m_VFXIsAlone || m_VFXIsAlone && !p_Instantiator.transform.GetChild(m_VFXChildOfChildNumber[p_NumberInTheList]).Find(m_VFXList[p_NumberInTheList].name))
                {
                    Vector3 l_VFXSpecificPosition = m_VFXSpecificPosition[p_NumberInTheList];
                    if (l_VFXSpecificPosition.x / Mathf.Abs(l_VFXSpecificPosition.x) == p_Instantiator.transform.GetChild(m_VFXChildOfChildNumber[p_NumberInTheList]).localScale.x / Mathf.Abs(p_Instantiator.transform.GetChild(m_VFXChildOfChildNumber[p_NumberInTheList]).localScale.x))
                    {
                        Debug.Log("aaa");
                        l_VFXSpecificPosition.x *= -1;
                    }
                    GameObject l_VFXInstiated = Instantiate(m_VFXList[p_NumberInTheList], p_Instantiator.transform.position + l_VFXSpecificPosition, Quaternion.Euler(Quaternion.identity.x + m_VFXSpecificRotation[p_NumberInTheList].x, Quaternion.identity.y + m_VFXSpecificRotation[p_NumberInTheList].y, Quaternion.identity.z + m_VFXSpecificRotation[p_NumberInTheList].z));
                    l_VFXInstiated.name = m_VFXList[p_NumberInTheList].name;
                    l_VFXInstiated.transform.parent = p_Instantiator.transform.GetChild(m_VFXChildOfChildNumber[p_NumberInTheList]);
                }
            }

        }
    }
    public void InstantiateAllVFX(GameObject p_Instantiator)
    {
        if (m_HasVFX)
        {
            for (int i = 0; i < m_VFXList.Length; i++)
            {
                if (!m_VFXHasToBeChild[i] && !m_VFXHasToBeChildOfChild[i])
                {
                    if (!m_VFXIsAlone || m_VFXIsAlone && FindObjectOfType<ParticleSystem>().gameObject.name != m_VFXList[i].name)
                    {
                        Vector3 l_VFXSpecificPosition = m_VFXSpecificPosition[i];
                        if (l_VFXSpecificPosition.x / Mathf.Abs(l_VFXSpecificPosition.x) != p_Instantiator.transform.localScale.x / Mathf.Abs(p_Instantiator.transform.localScale.x))
                        {
                            l_VFXSpecificPosition.x *= -1;
                        }
                        GameObject l_VFXInstiated = Instantiate(m_VFXList[i], p_Instantiator.transform.position + l_VFXSpecificPosition, Quaternion.Euler(Quaternion.identity.x + m_VFXSpecificRotation[i].x, Quaternion.identity.y + m_VFXSpecificRotation[i].y, Quaternion.identity.z + m_VFXSpecificRotation[i].z));
                        l_VFXInstiated.name = m_VFXList[i].name;
                    }
                }
                else if (m_VFXHasToBeChild[i])
                {
                    if (!m_VFXIsAlone || m_VFXIsAlone && !p_Instantiator.transform.Find(m_VFXList[i].name))
                    {
                        Vector3 l_VFXSpecificPosition = m_VFXSpecificPosition[i];
                        if (l_VFXSpecificPosition.x / Mathf.Abs(l_VFXSpecificPosition.x) != p_Instantiator.transform.localScale.x / Mathf.Abs(p_Instantiator.transform.localScale.x))
                        {
                            l_VFXSpecificPosition.x *= -1;
                        }
                        GameObject l_VFXInstiated = Instantiate(m_VFXList[i], p_Instantiator.transform.position + l_VFXSpecificPosition, Quaternion.Euler(Quaternion.identity.x + m_VFXSpecificRotation[i].x, Quaternion.identity.y + m_VFXSpecificRotation[i].y, Quaternion.identity.z + m_VFXSpecificRotation[i].z));
                        l_VFXInstiated.name = m_VFXList[i].name;
                        l_VFXInstiated.transform.parent = p_Instantiator.transform;
                    }
                }
                else if (m_VFXHasToBeChildOfChild[i])
                {
                    if (!m_VFXIsAlone || m_VFXIsAlone && !p_Instantiator.transform.GetChild(m_VFXChildOfChildNumber[i]).Find(m_VFXList[i].name))
                    {
                        Vector3 l_VFXSpecificPosition = m_VFXSpecificPosition[i];
                        if (l_VFXSpecificPosition.x / Mathf.Abs(l_VFXSpecificPosition.x) == p_Instantiator.transform.GetChild(m_VFXChildOfChildNumber[i]).localScale.x / Mathf.Abs(p_Instantiator.transform.GetChild(m_VFXChildOfChildNumber[i]).localScale.x))
                        {
                            l_VFXSpecificPosition.x *= -1;
                        }
                        GameObject l_VFXInstiated = Instantiate(m_VFXList[i], p_Instantiator.transform.position + l_VFXSpecificPosition, Quaternion.Euler(Quaternion.identity.x + m_VFXSpecificRotation[i].x, Quaternion.identity.y + m_VFXSpecificRotation[i].y, Quaternion.identity.z + m_VFXSpecificRotation[i].z));
                        l_VFXInstiated.name = m_VFXList[i].name;
                        l_VFXInstiated.transform.parent = p_Instantiator.transform.GetChild(m_VFXChildOfChildNumber[i]);
                    }
                }
            }
        }
    }

    public void StopVFX(GameObject p_Instantiator, int p_NumberInTheList)
    {
        ParticleSystem[] l_ParticleSystemInInstantiator = FindObjectsOfType<ParticleSystem>();
        for (int i = 0; i < l_ParticleSystemInInstantiator.Length; i++)
        {
            if(l_ParticleSystemInInstantiator[i].name == m_VFXList[p_NumberInTheList].name)
            {
                Destroy(l_ParticleSystemInInstantiator[i].gameObject);
            }
        }
    }

    public void StopAllVFX(GameObject p_Instantiator)
    {
        for(int n = 0; n < m_VFXList.Length; n++)
        {
            while (p_Instantiator.transform.Find(m_VFXList[n].name))
            {
                Transform l_ParticleSystemInInstantiator = p_Instantiator.transform.Find(m_VFXList[n].name);
                if(m_VFXHasToDisappearSmoothly)
                    StopVFXToDeath(l_ParticleSystemInInstantiator.GetComponent<ParticleSystem>());
                else
                    Destroy(l_ParticleSystemInInstantiator.gameObject);
                if (p_Instantiator.transform.Find(m_VFXList[n].name))
                {
                    return;
                }
            }
        }
        ParticleSystem[] l_ParticleSystemInScene = FindObjectsOfType<ParticleSystem>();
        for (int i = 0; i < l_ParticleSystemInScene.Length; i++)
        {
            for(int v = 0; v < m_VFXList.Length; v++)
            {
                if(l_ParticleSystemInScene[i].name == m_VFXList[v].name)
                {
                    if(m_VFXHasToDisappearSmoothly)
                        StopVFXToDeath(l_ParticleSystemInScene[i]);
                    else
                        Destroy(l_ParticleSystemInScene[i].gameObject);
                    break;
                }
            }
        }
    }

    private void StopVFXToDeath(ParticleSystem p_ParticleSystem)
    {
        p_ParticleSystem.Stop();
    }

    [Header("SFX")]
    [SerializeField]
    private bool m_HasSFX = false;
    [SerializeField]
    private AudioClip[] m_SFXList = null;
    public AudioClip[] SFXList { get { return m_SFXList; } }

    public void InstantiateAudioClip(int p_NumberInTheList)
    {
        if(m_HasSFX)
            FindObjectOfType<AudioSource>().PlayOneShot(m_SFXList[p_NumberInTheList]);
    }
    public void InstantiateAllAudioClip()
    {
        if(m_HasSFX)
            foreach (AudioClip l_AudioClip in m_SFXList)
                FindObjectOfType<AudioSource>().PlayOneShot(l_AudioClip);
    }

    public void StopAllAudioClip()
    {

    }


    [Header("VFX Animation")]
    [SerializeField]
    private bool m_HasVFXAnimation = false;
    [SerializeField]
    private GameObject[] m_VFXAnimationList = null;
    [SerializeField]
    private Vector3[] m_VFXAnimationSpecificPosition = null;
    [SerializeField]
    private Vector3[] m_VFXAnimationSpecificRotation = null;
    [SerializeField]
    private bool[] m_VFXAnimationHasToBeChild = null;
    [SerializeField]
    private bool[] m_VFXAnimationHasToBeChildOfChild = null;
    [SerializeField]
    private int[] m_VFXAnimationChildOfChildNumber = null;
    [SerializeField]
    private bool m_VFXAnimationIsAlone = false;


    public void InstantiateVFXAnimation(GameObject p_Instantiator, int p_NumberInTheList)
    {
        if (m_HasVFXAnimation)
        {
            if (!m_VFXAnimationHasToBeChild[p_NumberInTheList] && !m_VFXAnimationHasToBeChildOfChild[p_NumberInTheList])
            {
                if (!m_VFXAnimationIsAlone || m_VFXAnimationIsAlone && FindObjectOfType<FramePerfectAnimator>().gameObject.name != m_VFXAnimationList[p_NumberInTheList].name)
                {
                    Vector3 l_VFXAnimationSpecificPosition = m_VFXAnimationSpecificPosition[p_NumberInTheList];
                    if (l_VFXAnimationSpecificPosition.x / Mathf.Abs(l_VFXAnimationSpecificPosition.x) == p_Instantiator.transform.localScale.x / Mathf.Abs(p_Instantiator.transform.GetChild(0).localScale.x))
                        l_VFXAnimationSpecificPosition.x *= -1;

                    GameObject l_VFXAnimation = Instantiate(m_VFXAnimationList[p_NumberInTheList], p_Instantiator.transform.position + l_VFXAnimationSpecificPosition, Quaternion.Euler(Quaternion.identity.x + m_VFXAnimationSpecificRotation[p_NumberInTheList].x, Quaternion.identity.y + m_VFXAnimationSpecificRotation[p_NumberInTheList].y, Quaternion.identity.z + m_VFXAnimationSpecificRotation[p_NumberInTheList].z));
                    l_VFXAnimation.name = m_VFXAnimationList[p_NumberInTheList].name;
                }
            }
            else if (m_VFXAnimationHasToBeChild[p_NumberInTheList])
            {
                if (!m_VFXAnimationIsAlone || m_VFXAnimationIsAlone && !p_Instantiator.transform.Find(m_VFXAnimationList[p_NumberInTheList].name))
                {
                    Vector3 l_VFXAnimationSpecificPosition = m_VFXAnimationSpecificPosition[p_NumberInTheList];
                    if (l_VFXAnimationSpecificPosition.x / Mathf.Abs(l_VFXAnimationSpecificPosition.x) == p_Instantiator.transform.localScale.x / Mathf.Abs(p_Instantiator.transform.GetChild(0).localScale.x))
                        l_VFXAnimationSpecificPosition.x *= -1;

                    GameObject l_VFXAnimation = Instantiate(m_VFXAnimationList[p_NumberInTheList], p_Instantiator.transform.position + l_VFXAnimationSpecificPosition, Quaternion.Euler(Quaternion.identity.x + m_VFXAnimationSpecificRotation[p_NumberInTheList].x, Quaternion.identity.y + m_VFXAnimationSpecificRotation[p_NumberInTheList].y, Quaternion.identity.z + m_VFXAnimationSpecificRotation[p_NumberInTheList].z));
                    l_VFXAnimation.transform.parent = p_Instantiator.transform;
                    l_VFXAnimation.name = m_VFXAnimationList[p_NumberInTheList].name;
                }
            }
            else if (m_VFXAnimationHasToBeChildOfChild[p_NumberInTheList])
            {
                if (!m_VFXAnimationIsAlone || m_VFXAnimationIsAlone && !p_Instantiator.transform.GetChild(m_VFXAnimationChildOfChildNumber[p_NumberInTheList]).Find(m_VFXAnimationList[p_NumberInTheList].name))
                {
                    Vector3 l_VFXAnimationSpecificPosition = m_VFXAnimationSpecificPosition[p_NumberInTheList];
                    if (l_VFXAnimationSpecificPosition.x / Mathf.Abs(l_VFXAnimationSpecificPosition.x) == p_Instantiator.transform.GetChild(m_VFXAnimationChildOfChildNumber[p_NumberInTheList]).localScale.x / Mathf.Abs(p_Instantiator.transform.GetChild(m_VFXAnimationChildOfChildNumber[p_NumberInTheList]).localScale.x))
                        l_VFXAnimationSpecificPosition.x *= -1;

                    GameObject l_VFXAnimation = Instantiate(m_VFXAnimationList[p_NumberInTheList], p_Instantiator.transform.position + l_VFXAnimationSpecificPosition, Quaternion.Euler(Quaternion.identity.x + m_VFXAnimationSpecificRotation[p_NumberInTheList].x, Quaternion.identity.y + m_VFXAnimationSpecificRotation[p_NumberInTheList].y, Quaternion.identity.z + m_VFXAnimationSpecificRotation[p_NumberInTheList].z));
                    l_VFXAnimation.transform.parent = p_Instantiator.transform.GetChild(m_VFXAnimationChildOfChildNumber[p_NumberInTheList]);
                    l_VFXAnimation.name = m_VFXAnimationList[p_NumberInTheList].name;
                }
            }
        }
    }

    public void InstantiateAllVFXAnimation(GameObject p_Instantiator)
    {
        if (m_HasVFXAnimation)
        {
            for(int i = 0; i < m_VFXAnimationList.Length; i++)
            {
                if (!m_VFXAnimationHasToBeChild[i] && !m_VFXAnimationHasToBeChildOfChild[i])
                {
                    FramePerfectAnimator[] l_FramePerfectAnimatorInScene = FindObjectsOfType<FramePerfectAnimator>();
                    int l_ThisFeedbacksInScene = 0;
                    for (int b = 0; b < l_FramePerfectAnimatorInScene.Length; b++)
                    {
                        if (l_FramePerfectAnimatorInScene[b].gameObject.name == m_VFXAnimationList[i].name)
                            l_ThisFeedbacksInScene += 1;
                    }
                    if (!m_VFXAnimationIsAlone || m_VFXAnimationIsAlone && l_ThisFeedbacksInScene == 0)
                    {
                        Vector3 l_VFXAnimationSpecificPosition = m_VFXAnimationSpecificPosition[i];
                        if (l_VFXAnimationSpecificPosition.x / Mathf.Abs(l_VFXAnimationSpecificPosition.x) == p_Instantiator.transform.localScale.x / Mathf.Abs(p_Instantiator.transform.GetChild(0).localScale.x))
                            l_VFXAnimationSpecificPosition.x *= -1;

                        GameObject l_VFXAnimation = Instantiate(m_VFXAnimationList[i], p_Instantiator.transform.position + l_VFXAnimationSpecificPosition, Quaternion.Euler(Quaternion.identity.x + m_VFXAnimationSpecificRotation[i].x, Quaternion.identity.y + m_VFXAnimationSpecificRotation[i].y, Quaternion.identity.z + m_VFXAnimationSpecificRotation[i].z));
                        l_VFXAnimation.name = m_VFXAnimationList[i].name;
                    }
                }
                else if (m_VFXAnimationHasToBeChild[i])
                {
                    if (!m_VFXAnimationIsAlone || m_VFXAnimationIsAlone && !p_Instantiator.transform.Find(m_VFXAnimationList[i].name))
                    {
                        Vector3 l_VFXAnimationSpecificPosition = m_VFXAnimationSpecificPosition[i];
                        if(l_VFXAnimationSpecificPosition.x / Mathf.Abs(l_VFXAnimationSpecificPosition.x) == p_Instantiator.transform.localScale.x / Mathf.Abs(p_Instantiator.transform.GetChild(0).localScale.x))
                            l_VFXAnimationSpecificPosition.x *= -1;

                        GameObject l_VFXAnimation = Instantiate(m_VFXAnimationList[i], p_Instantiator.transform.position + l_VFXAnimationSpecificPosition, Quaternion.Euler(Quaternion.identity.x + m_VFXAnimationSpecificRotation[i].x, Quaternion.identity.y + m_VFXAnimationSpecificRotation[i].y, Quaternion.identity.z + m_VFXAnimationSpecificRotation[i].z));
                        l_VFXAnimation.transform.parent = p_Instantiator.transform;
                        l_VFXAnimation.name = m_VFXAnimationList[i].name;
                    }
                }
                else if (m_VFXAnimationHasToBeChildOfChild[i])
                {
                    if (!m_VFXAnimationIsAlone || m_VFXAnimationIsAlone && !p_Instantiator.transform.GetChild(m_VFXAnimationChildOfChildNumber[i]).Find(m_VFXAnimationList[i].name))
                    {
                        Vector3 l_VFXAnimationSpecificPosition = m_VFXAnimationSpecificPosition[i];
                        if (l_VFXAnimationSpecificPosition.x / Mathf.Abs(l_VFXAnimationSpecificPosition.x) == p_Instantiator.transform.GetChild(m_VFXAnimationChildOfChildNumber[i]).localScale.x / Mathf.Abs(p_Instantiator.transform.GetChild(m_VFXAnimationChildOfChildNumber[i]).localScale.x))
                        {
                            l_VFXAnimationSpecificPosition.x *= -1;
                        }

                        GameObject l_VFXAnimation = Instantiate(m_VFXAnimationList[i], p_Instantiator.transform.GetChild(m_VFXAnimationChildOfChildNumber[i]));
                        l_VFXAnimation.transform.position = p_Instantiator.transform.GetChild(m_VFXAnimationChildOfChildNumber[i]).transform.position + l_VFXAnimationSpecificPosition;
                        l_VFXAnimation.transform.rotation = Quaternion.Euler(Quaternion.identity.x + m_VFXAnimationSpecificRotation[i].x, Quaternion.identity.y + m_VFXAnimationSpecificRotation[i].y, Quaternion.identity.z + m_VFXAnimationSpecificRotation[i].z);
                        l_VFXAnimation.name = m_VFXAnimationList[i].name;
                    }
                }
            }
        }
    }

    public void StopVFXAnimation(GameObject p_Instantiator, int p_NumberInTheList)
    {
        FramePerfectAnimator[] l_FPAInInstantiator = p_Instantiator.GetComponentsInChildren<FramePerfectAnimator>();
        for (int i = 0; i < l_FPAInInstantiator.Length; i++)
        {
            if (l_FPAInInstantiator[i].name == m_VFXAnimationList[p_NumberInTheList].name)
            {
                Destroy(l_FPAInInstantiator[p_NumberInTheList].gameObject);
            }
        }
    }

    public void StopAllVFXAnimation(GameObject p_Instantiator)
    {
        FramePerfectAnimator[] l_FPAInInstantiator = p_Instantiator.GetComponentsInChildren<FramePerfectAnimator>();
        for (int i = 0; i < l_FPAInInstantiator.Length; i++)
        {
            for (int v = 0; v < m_VFXAnimationList.Length; v++)
            {
                if (l_FPAInInstantiator[i].name == m_VFXAnimationList[v].name)
                {
                    Destroy(l_FPAInInstantiator[i].gameObject);
                    break;
                }
            }
        }
    }

    [Header("Shake")]
    [SerializeField]
    private bool m_HasToShake = false;
    public bool HasToShake { get { return m_HasToShake; } }
    [SerializeField]
    private bool m_ObjectToShakeIsCamera = false;
    [SerializeField]
    private float m_ShakeIntesity = 0.1f;
    public float ShakeIntesity { get { return m_ShakeIntesity; } }
    [SerializeField]
    private float m_ShakeDuration = 0.1f;
    public float ShakeDuration { get { return m_ShakeDuration; } }

    public void Shaking(GameObject p_ObjectToShake)
    {
        if (m_HasToShake)
        {
            if(m_ObjectToShakeIsCamera)
            {
                Camera l_Camera = FindObjectOfType<Camera>();
                Shake l_Shake = FindObjectOfType<Shake>();
                l_Shake.StartCoroutine(l_Shake.CreateShake(m_ShakeIntesity, m_ShakeDuration, l_Camera.gameObject));
            }
            else
            {
                Shake l_Shake = FindObjectOfType<Shake>();
                l_Shake.StartCoroutine(l_Shake.CreateShake(m_ShakeIntesity, m_ShakeDuration, p_ObjectToShake));
            }
        }
    }

    [Header("Blink")]
    [SerializeField]
    private bool m_HasToBlink = false;
    public bool HasToBlink { get { return m_HasToBlink; } }
    [SerializeField, Range(0f, 1f)]
    private float m_BlinkIntensity = 0.1f;
    public float BlinkIntensity { get { return m_BlinkIntensity; } }
    [SerializeField]
    private float m_BlinkDuration = 0.1f;
    public float BlinkDuration { get { return m_BlinkDuration; } }
    [SerializeField]
    private float m_BlinkSpeed = 0.5f;

    public void Blink(GameObject p_ObjectToBlink)
    {
        if (m_HasToBlink)
        {
            p_ObjectToBlink.GetComponent<Blink>().StartCoroutine(p_ObjectToBlink.GetComponent<Blink>().Blinking(p_ObjectToBlink, m_BlinkDuration, m_BlinkSpeed, m_BlinkIntensity));
        }
    }

    [Header("Freeze Frame")]
    [SerializeField]
    private bool m_HasToFreezeFrame = false;
    public bool HasToFreezeFrame { get { return m_HasToFreezeFrame; } }
    [SerializeField]
    private float m_FreezeFrameDuration = 0.1f;
    public float FreezeFrameDuration { get { return m_FreezeFrameDuration; } }


    [Header("Vibration")]
    [SerializeField]
    private bool m_HasToVibrate = false;
    public bool HasToVibrate { get { return m_HasToVibrate; } }
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
        Shaking(p_Initiator);
    }

    public void StopFeedback(GameObject p_Initiator)
    {
        StopAllVFX(p_Initiator);
        StopAllVFXAnimation(p_Initiator);
    }
}
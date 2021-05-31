using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
    [SerializeField]
    private List<AudioMixerGroup> m_AudioGroup = new List<AudioMixerGroup>();
    [SerializeField]
    private List<Slider> m_AudioSliders = new List<Slider>();
    private int m_CurrentSlider = 0;

    public void JoyStickInput(InputAction.CallbackContext p_Context)
    {
        if (p_Context.started)
        {
            Vector2 l_Direction = p_Context.ReadValue<Vector2>();
            float l_Angle = Vector3.Angle(Vector3.right, l_Direction);
            if (l_Angle > 135.0f)
            {
                //Left
                m_AudioSliders[m_CurrentSlider].value = m_AudioSliders[m_CurrentSlider].value - 1;
                m_AudioGroup[m_CurrentSlider].audioMixer.SetFloat(m_AudioSliders[m_CurrentSlider].gameObject.name, m_AudioSliders[m_CurrentSlider].value);
            }
            else if (l_Angle > 45.0f && l_Direction.y > 0)
            {
                //Up
                if (m_CurrentSlider == 0)
                {
                    m_CurrentSlider = m_AudioSliders.Count - 1;
                }
                else
                {
                    m_CurrentSlider = m_CurrentSlider - 1;
                }
            }
            else if (l_Angle > 45.0f && l_Direction.y < 0)
            {
                //Down
                if (m_CurrentSlider == m_AudioSliders.Count - 1)
                {
                    m_CurrentSlider = 0;
                }
                else
                {
                    m_CurrentSlider = m_CurrentSlider + 1;
                }
            }
            else
            {
                //Right
                m_AudioSliders[m_CurrentSlider].value = m_AudioSliders[m_CurrentSlider].value + 1;
                m_AudioGroup[m_CurrentSlider].audioMixer.SetFloat(m_AudioSliders[m_CurrentSlider].gameObject.name, m_AudioSliders[m_CurrentSlider].value);
            }
        }
    }
}

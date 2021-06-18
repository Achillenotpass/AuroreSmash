using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarFeedback : MonoBehaviour
{
    [SerializeField]
    private Slider m_HealthBar = null;
    private Slider m_RedHealthBar = null;

    private void Awake()
    {
        m_RedHealthBar = GetComponent<Slider>();
    }
    private void Start()
    {
        m_RedHealthBar.maxValue = m_HealthBar.maxValue;
        m_RedHealthBar.value = m_HealthBar.value;
    }


    public void HealthChanged(float p_NewValue)
    {
        StopAllCoroutines();
        StartCoroutine(DecreaseRedHealthBarOverTime(1.5f, p_NewValue));
    }
    private IEnumerator DecreaseRedHealthBarOverTime(float p_Time, float p_NewValue)
    {
        float l_Timer = 0.0f;
        float m_LastValue = m_RedHealthBar.value;

        while (p_NewValue != m_RedHealthBar.value)
        {
            l_Timer = l_Timer + Time.deltaTime;
            m_RedHealthBar.value = Mathf.Lerp(m_LastValue, p_NewValue, l_Timer / p_Time);

            yield return null;
        }
    }
}

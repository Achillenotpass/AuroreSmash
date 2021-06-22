using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayValue : MonoBehaviour
{
    private Slider m_Slider = null;
    [SerializeField]
    private Text m_DisplayedValue = null;

    private void Awake()
    {
        m_Slider = GetComponent<Slider>();
    }
    private void Update()
    {
        m_DisplayedValue.text = m_Slider.value.ToString();
    }
}

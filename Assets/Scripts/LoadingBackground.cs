using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LoadingBackground : MonoBehaviour
{
    [SerializeField]
    private float m_MovingTime = 0.1f;
    [SerializeField]
    private GameObject m_OutsidePosition = null;
    [SerializeField]
    private GameObject m_InsidePosition = null;

    private void Awake()
    {
        GetComponent<RectTransform>().DOAnchorPos(m_OutsidePosition.GetComponent<RectTransform>().anchoredPosition, m_MovingTime);
    }
    public void AppearLoadingBackground()
    {
        GetComponent<RectTransform>().DOAnchorPos(m_InsidePosition.GetComponent<RectTransform>().anchoredPosition, m_MovingTime);
    }
}

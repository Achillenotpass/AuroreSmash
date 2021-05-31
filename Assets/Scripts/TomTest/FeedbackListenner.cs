using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackListenner : MonoBehaviour
{
    [SerializeField]
    private SO_FeedbacksList m_FeedbackList = null;

    public SO_FeedbacksList FeedbackList
    {
        get
        {
            return m_FeedbackList;
        }
    }
}

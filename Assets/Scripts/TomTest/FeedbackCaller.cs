using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackCaller : MonoBehaviour
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

    public void FeedbackCalling(SO_Feedback p_Feedback ,GameObject p_CallerInitiator)
    {
        p_Feedback.PlayFeedback(p_CallerInitiator);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackCaller : MonoBehaviour
{
    public void CallFeedback(SO_Feedback p_Feedback)
    {
        p_Feedback.PlayFeedback(this.gameObject);
    }
}
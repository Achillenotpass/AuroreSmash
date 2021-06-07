using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackCallerInitiator : MonoBehaviour
{
    public void FeedbackInitialCalling(SO_Feedback p_Feedback)
    {
        FindObjectOfType<FeedbackCaller>().FeedbackCalling(p_Feedback, gameObject);
    }
}

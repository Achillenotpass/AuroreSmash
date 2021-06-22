using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFeedbacksList", menuName = "ScriptableObjects/Gameplay/Feedbacks/FeedbacksList")]
public class SO_FeedbacksList : ScriptableObject
{
    [SerializeField]
    private SO_Feedback[] m_FeedbackArray = null;

    public SO_Feedback[] FeedbackArray
    {
        get { return m_FeedbackArray; }
    }
}
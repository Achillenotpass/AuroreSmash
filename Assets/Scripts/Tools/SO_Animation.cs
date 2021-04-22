using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimation", menuName = "ScriptableObjects/Graphs/NewAnimation")]
public class SO_Animation : ScriptableObject
{
    private int m_EndFrame = 0;
    public int EndFrame { get { return m_EndFrame; } }
    [SerializeField]
    private List<AnimationFrame> m_AnimationFrames = new List<AnimationFrame>();
    public List<AnimationFrame> AnimationFrames { get { return m_AnimationFrames; } }
    [SerializeField]
    private int m_LastFrameDuration = 0;

    public void SetGlobalFramePosition()
    {
        if (m_AnimationFrames.Count != 0)
        {
            for (int i = 0; i < m_AnimationFrames.Count; i++)
            {
                if (i == 0)
                {
                    m_AnimationFrames[i].m_GlobalFramePosition = m_AnimationFrames[i].m_FramesAfterLastSprite;
                }
                else
                {
                    m_AnimationFrames[i].m_GlobalFramePosition = m_AnimationFrames[i - 1].m_GlobalFramePosition + m_AnimationFrames[i].m_FramesAfterLastSprite;
                }
                m_EndFrame = m_AnimationFrames[i].m_GlobalFramePosition + m_LastFrameDuration;
            }
        }
    }
}

[System.Serializable]
public class AnimationFrame
{
    public Sprite m_FrameSprite = null;
    public int m_FramesAfterLastSprite = 0;
    [System.NonSerialized]
    public int m_GlobalFramePosition = 0;
}

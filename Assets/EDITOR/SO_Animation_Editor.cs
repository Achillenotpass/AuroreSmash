using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SO_Animation))]
public class SO_Animation_Editor : Editor
{
    private SO_Animation m_TargetAnimation = null;
    private string m_FrameDelay = "Delay between sprite in whole frames";
    private List<Sprite> m_AnimationSprites = new List<Sprite>();
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Set all sprite delays to"))
        {

            if (int.TryParse(m_FrameDelay, out int l_IntFrameDelay))
            {
                SetAllSpriteDelays(l_IntFrameDelay);
            }
            else
            {
                Debug.Log("String wasn't a whole number");
            }

        }
        m_FrameDelay = GUILayout.TextField(m_FrameDelay);
        GUILayout.EndHorizontal();
        //Ajouter tableau de sprites
    }

    public void SetAllSpriteDelays(int p_DelayInFrame)
    {
        m_TargetAnimation = (SO_Animation)target;
        foreach (AnimationFrame l_AnimationFrame in m_TargetAnimation.AnimationFrames)
        {
            l_AnimationFrame.m_FramesAfterLastSprite = p_DelayInFrame;
        }

    }

}

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
        EditorGUILayout.Space();
        if (GUILayout.Button("Set all dragged sprites") && m_AnimationSprites.Count != 0)
        {
            m_TargetAnimation = (SO_Animation)target;
            m_TargetAnimation.AnimationFrames.Clear();
            for (int i = 0; i < m_AnimationSprites.Count; i++)
            {
                m_TargetAnimation.AnimationFrames.Add(new AnimationFrame());
                m_TargetAnimation.AnimationFrames[i].m_FrameSprite = m_AnimationSprites[i];
            }
            EditorUtility.SetDirty((SO_Animation)target);
        }
        DropAreaGUI();
    }

    public void DropAreaGUI()
    {
        Event evt = Event.current;
        Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(drop_area, "Add Trigger");

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!drop_area.Contains(evt.mousePosition))
                {
                    return;
                }

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    m_AnimationSprites.Clear();
                    foreach (Sprite l_DraggedSprite in DragAndDrop.objectReferences)
                    {
                        if (l_DraggedSprite != null)
                        {
                            m_AnimationSprites.Add(l_DraggedSprite);
                        }
                    }
                }
                break;
        }
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

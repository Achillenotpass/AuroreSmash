using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AttackCreator))]
public class AttackCreator_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Previous frame"))
            {
                (target as AttackCreator).GoToPreviousFrame();
            }
            if (GUILayout.Button("Next frame"))
            {
                (target as AttackCreator).GoToNextFrame();
            }
        }
        GUILayout.EndHorizontal();
    }
}

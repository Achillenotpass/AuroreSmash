using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour
    where T : Component
{
    private static T s_Instance = null;

    public static T Instance
    {
        get
        {
            if (s_Instance == null)
            {
                T[] l_ComponentsInScene = FindObjectsOfType<T>();
                if (l_ComponentsInScene.Length > 0)
                {
                    s_Instance = l_ComponentsInScene[0];
                }
                for (int i = 1; i < l_ComponentsInScene.Length; i++)
                {
                    Destroy(l_ComponentsInScene[i]);
                }
            }

            if (s_Instance == null)
            {
                GameObject l_Obj = new GameObject("Update Manager", new System.Type[] { typeof(T) });
                s_Instance = l_Obj.AddComponent<T>();
            }
            return s_Instance;
        }
    }
}

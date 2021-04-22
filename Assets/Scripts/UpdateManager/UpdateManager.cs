using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    private static Dictionary<SO_UpdateLayerSettings, UpdateManager> m_UpdateManager = new Dictionary<SO_UpdateLayerSettings, UpdateManager>();

    private SO_UpdateLayerSettings m_Settings = null;

    private List<IUpdateUser> m_Users = new List<IUpdateUser>();

    private void FixedUpdate()
    {
        if (m_Settings.UpdateType != EUpdateType.FixedUpdate)
            return;

        for (int i = 0; i < m_Users.Count; i++)
        {
            m_Users[i].CustomUpdate(Time.fixedDeltaTime * m_Settings.Multiplier);
        }
        //foreach (IUpdateUser user in m_Users)
        //{
        //    user.CustomUpdate(Time.fixedDeltaTime * m_Settings.Multiplier);
        //}
        
    }

    private void Update()
    {
        if (m_Settings.UpdateType == EUpdateType.FixedUpdate)
            return;

        Debug.Log("Update " + name);

        foreach (IUpdateUser user in m_Users)
        {
            user.CustomUpdate(Time.deltaTime * m_Settings.Multiplier);
        }
        
    }

    public static void Bind(SO_UpdateLayerSettings p_Settings, IUpdateUser p_User)
    {
        GetInstance(p_Settings).Bind(p_User);
    }

    public void Bind(IUpdateUser p_User)
    {
        if (!m_Users.Contains(p_User))
        {
            m_Users.Add(p_User);
        }
    }

    public static void Unbind(SO_UpdateLayerSettings p_Settings, IUpdateUser p_User)
    {
        GetInstance(p_Settings, true)?.Unbind(p_User);
    }

    public void Unbind(IUpdateUser p_User)
    {
        m_Users.Remove(p_User);
    }

    private static UpdateManager GetInstance(SO_UpdateLayerSettings p_Settings, bool p_OnUnbind = false)
    {
        UpdateManager l_UpdateManager = null;
        if (m_UpdateManager.TryGetValue(p_Settings, out l_UpdateManager))
        {
            if (l_UpdateManager != null)
            {
                return m_UpdateManager[p_Settings];
            }
            m_UpdateManager.Remove(p_Settings);
        }

        if (p_OnUnbind)
            return null;

        GameObject l_Obj = new GameObject($"Update Manager ({p_Settings.name})");
        l_UpdateManager = l_Obj.AddComponent<UpdateManager>();
        l_UpdateManager.m_Settings = p_Settings;
        m_UpdateManager.Add(p_Settings, l_UpdateManager);
        return l_UpdateManager;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUpdateLayerSettings", menuName = "_Game/Update Layer Settings")]
public class SO_UpdateLayerSettings : ScriptableObject
{
    [SerializeField] private EUpdateType m_UpdateType = EUpdateType.Update;
    [SerializeField] private float m_CustomInterval = 1f;
    [SerializeField] private float m_Multiplier = 1;

    public void Bind(IUpdateUser p_User)
    {
        UpdateManager.Bind(this, p_User);
    }

    public void Unbind(IUpdateUser p_User)
    {
        UpdateManager.Unbind(this, p_User);
    }

    public EUpdateType UpdateType => m_UpdateType;
    public float CustomInterval => m_CustomInterval;
    public float Multiplier => m_Multiplier;
}

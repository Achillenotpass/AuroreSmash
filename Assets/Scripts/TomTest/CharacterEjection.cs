using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEjection : MonoBehaviour, IUpdateUser
{
    #region CustomUpdate
    [SerializeField]
    private SO_UpdateLayerSettings m_UpdateSettings = null;
    private void OnEnable()
    {
        m_UpdateSettings.Bind(this);
    }
    private void OnDisable()
    {
        m_UpdateSettings.Unbind(this);
    }
    #endregion
    #region Variables
    #endregion

    #region Awake/Start
    private void Awake()
    {
    }

    private void Start()
    {
    }
    #endregion

    #region Update
    public void CustomUpdate(float p_DeltaTime)
    {

    }
    #endregion


    public void CalculateEjection(float p_EjectionPower, float p_EjectionAngle)
    {
    }
}
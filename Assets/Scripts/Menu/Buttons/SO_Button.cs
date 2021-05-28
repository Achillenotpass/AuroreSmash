using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SO_Button : ScriptableObject
{
    #region Variables
    [SerializeField]
    private GameObject m_ButtonPrefab = null;
    public GameObject ButtonPrefab { get { return m_ButtonPrefab; } }
    #endregion
}

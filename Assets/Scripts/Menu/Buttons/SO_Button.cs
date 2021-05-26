using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewButton", menuName = "ScriptableObjects/Menus/NewButton")]
public class SO_Button : ScriptableObject
{
    #region Variables
    [SerializeField]
    private GameObject m_ButtonPrefab = null;
    #endregion
}

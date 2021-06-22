using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelMenu : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private GameObject m_OffSetButton = null;
    [SerializeField]
    private GameObject m_WheelPivot = null;
    [SerializeField]
    private List<SO_Button> m_Buttons = new List<SO_Button>();
    #endregion
    #region Awake/Start/Update
    private void Start()
    {
        //if (m_Buttons.Count == 1)
        //{
        //    GameObject l_NewButton = Instantiate(m_Buttons[0].ButtonPrefab, m_OffSetButton.transform.position, m_OffSetButton.transform.rotation);
        //}
        //else if (m_Buttons.Count > 1)
        //{
        //    float l_AngleBetweenButtons = 360.0f / (float)m_Buttons.Count;
        //    for (int i = 0; i < m_Buttons.Count; i++)
        //    {
        //        GameObject l_NewButton = Instantiate(m_Buttons[0].ButtonPrefab, m_OffSetButton.transform.position, m_OffSetButton.transform.rotation);
        //        l_NewButton.transform.SetParent(m_WheelPivot.transform);

        //        m_OffSetButton.GetComponent<RectTransform>().RotateAround(m_WheelPivot.transform.position, Vector3.forward, l_AngleBetweenButtons);
        //        //m_OffSetButton.transform.RotateAround(m_WheelPivot.transform.position, Vector3.forward, l_AngleBetweenButtons);
        //    }
        //}
        //m_OffSetButton.SetActive(false);
    }
    #endregion
    #region Inputs
    #endregion
    #region Functions
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapSelection : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private List<string> m_MapNames = new List<string>();
    [SerializeField]
    private List<GameObject> m_MapBackgrounds = new List<GameObject>();
    private int m_CurrentMap = 0;
    [SerializeField]
    private Text m_MapName = null;
    [Header("UI display")]
    [SerializeField]
    private GameObject m_Buttons = null;
    [SerializeField]
    private GameObject m_WheelCenter = null;
    #endregion

    #region Awake/Start/Update
    private void Start()
    {
        m_MapName.text = m_MapNames[m_CurrentMap];
        DisplayMapBackground(m_CurrentMap);
    }
    #endregion

    #region Inputs
    public void JoysticksInput (InputAction.CallbackContext p_Context)
    {
        if (p_Context.started)
        {
            if (Vector2.Dot(p_Context.ReadValue<Vector2>().normalized, Vector2.right) > 0.2f)
            {
                m_CurrentMap = m_CurrentMap + 1;
                m_Buttons.transform.RotateAround(m_WheelCenter.transform.position, Vector3.forward, -45.0f);
                if (m_CurrentMap == m_MapNames.Count)
                {
                    m_CurrentMap = 0;
                    m_Buttons.transform.RotateAround(m_WheelCenter.transform.position, Vector3.forward, 45.0f);
                    m_Buttons.transform.RotateAround(m_WheelCenter.transform.position, Vector3.forward, 45.0f);
                }
            }
            else if (Vector2.Dot(p_Context.ReadValue<Vector2>().normalized, Vector2.right) < 0.2f)
            {
                m_CurrentMap = m_CurrentMap - 1;
                m_Buttons.transform.RotateAround(m_WheelCenter.transform.position, Vector3.forward, 45.0f);
                if (m_CurrentMap < 0)
                {
                    m_CurrentMap = m_MapNames.Count - 1;
                    m_Buttons.transform.RotateAround(m_WheelCenter.transform.position, Vector3.forward, -45.0f);
                    m_Buttons.transform.RotateAround(m_WheelCenter.transform.position, Vector3.forward, -45.0f);
                }
            }
            DisplayMapBackground(m_CurrentMap);
            m_MapName.text = m_MapNames[m_CurrentMap];
        }
    }
    public void SelectInput(InputAction.CallbackContext p_Context)
    {
        SceneManager.LoadScene(m_MapNames[m_CurrentMap]);
    }
    public void ReturnInput(InputAction.CallbackContext p_Context)
    {
        SceneManager.LoadScene("CharacterSelection");
    }
    #endregion

    #region Functions
    public void DisplayMapBackground(int p_CurrentMap)
    {
        foreach (GameObject p_Background in m_MapBackgrounds)
        {
            p_Background.SetActive(false);
        }
        m_MapBackgrounds[p_CurrentMap].SetActive(true);
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapSelection : MonoBehaviour
{
    #region Variables
    private bool m_InAnimation = false;
    [SerializeField]
    private float m_AngleIncrement = 45.0f;
    [SerializeField]
    private float m_RotationTime = 45.0f;
    [SerializeField]
    private List<string> m_MapNames = new List<string>();
    [SerializeField]
    private List<GameObject> m_MapBackgrounds = new List<GameObject>();
    private int m_CurrentMap = 0;
    [Header("UI display")]
    [SerializeField]
    private GameObject m_Buttons = null;
    [SerializeField]
    private GameObject m_WheelCenter = null;
    #endregion

    #region Awake/Start/Update
    private void Start()
    {
        DisplayMapBackground(m_CurrentMap);
    }
    #endregion

    #region Inputs
    public void JoysticksInput (InputAction.CallbackContext p_Context)
    {
        if (p_Context.started && !m_InAnimation)
        {
            if (Vector2.Dot(p_Context.ReadValue<Vector2>().normalized, Vector2.right) > 0.2f)
            {
                m_CurrentMap = m_CurrentMap + 1;
                //m_Buttons.transform.RotateAround(m_WheelCenter.transform.position, Vector3.forward, -m_AngleIncrement);
                if (m_CurrentMap == m_MapNames.Count)
                {
                    m_CurrentMap = 0;
                    StartCoroutine(RotateOverTimer(m_AngleIncrement, m_RotationTime));
                    //m_Buttons.transform.RotateAround(m_WheelCenter.transform.position, Vector3.forward, m_AngleIncrement);
                    //m_Buttons.transform.RotateAround(m_WheelCenter.transform.position, Vector3.forward, m_AngleIncrement);
                }
                else
                {
                    StartCoroutine(RotateOverTimer(-m_AngleIncrement, m_RotationTime));
                }
            }
            else if (Vector2.Dot(p_Context.ReadValue<Vector2>().normalized, Vector2.right) < 0.2f)
            {
                m_CurrentMap = m_CurrentMap - 1;
                //m_Buttons.transform.RotateAround(m_WheelCenter.transform.position, Vector3.forward, m_AngleIncrement);
                if (m_CurrentMap < 0)
                {
                    m_CurrentMap = m_MapNames.Count - 1;
                    StartCoroutine(RotateOverTimer(-m_AngleIncrement, m_RotationTime));
                    //m_Buttons.transform.RotateAround(m_WheelCenter.transform.position, Vector3.forward, -m_AngleIncrement);
                    //m_Buttons.transform.RotateAround(m_WheelCenter.transform.position, Vector3.forward, -m_AngleIncrement);
                }
                else
                {
                    StartCoroutine(RotateOverTimer(m_AngleIncrement, m_RotationTime));
                }
            }
            DisplayMapBackground(m_CurrentMap);
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
    public IEnumerator RotateOverTimer(float p_RotationAngle, float p_RotationTime)
    {
        m_InAnimation = true;

        float l_CurrentTimer = 0.0f;
        float l_RotationEffectued = 0.0f;
        while (l_CurrentTimer < p_RotationTime)
        {
            float l_AngleIncrement = 0.0f;

            l_AngleIncrement = Mathf.Lerp(0.0f, p_RotationAngle, l_CurrentTimer / p_RotationTime);

            m_Buttons.transform.RotateAround(m_WheelCenter.transform.position, Vector3.forward, l_AngleIncrement - l_RotationEffectued);
            l_RotationEffectued = l_AngleIncrement;
            l_CurrentTimer = l_CurrentTimer + Time.deltaTime;

            yield return null;
        }
        m_InAnimation = false;
    }
    #endregion
}

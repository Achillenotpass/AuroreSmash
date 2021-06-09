using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    #region Variables
    private bool m_InAnimation = false;
    [SerializeField]
    private List<string> m_MapNames = new List<string>();
    [SerializeField]
    private List<GameObject> m_MapBackgrounds = new List<GameObject>();
    private int m_CurrentButton = 0;
    [SerializeField]
    private Text m_MapName = null;
    [Header("UI display")]
    [SerializeField]
    private GameObject m_Buttons = null;
    [SerializeField]
    private GameObject m_WheelCenter = null;
    [SerializeField]
    private float m_AngleIncrement = 90.0f;
    [SerializeField]
    private float m_RotationTime = 0.25f;
    private MainMenuState m_MainMenuState = MainMenuState.OnMainWheel;
    [Header("Options sub-menu")]
    [SerializeField]
    private GameObject m_OptionsSubMenu = null;
    private int m_CurrentOptionButton = 0;
    [SerializeField]
    private List<GameObject> m_OptionSelectionFeedback = null;
    [SerializeField]
    private PlayerInput m_MainMenuInput = null;
    [SerializeField]
    private GameObject m_AudioSettingsCanvas = null;
    [SerializeField]
    private GameObject m_VideoSettingsCanvas = null;
    #endregion

    #region Awake/Start/Update
    private void Start()
    {
        m_MapName.text = m_MapNames[m_CurrentButton];
        DisplayMapBackground(m_CurrentButton);
    }
    #endregion

    #region Inputs
    public void JoysticksInput(InputAction.CallbackContext p_Context)
    {
        if (p_Context.started && !m_InAnimation)
        {
            switch (m_MainMenuState)
            {
                case MainMenuState.OnMainWheel:
                    if (Vector2.Dot(p_Context.ReadValue<Vector2>().normalized, Vector2.right) > 0.2f)
                    {
                        m_CurrentButton = m_CurrentButton + 1;
                        if (m_CurrentButton == m_MapNames.Count)
                        {
                            m_CurrentButton = 0;
                            StartCoroutine(RotateOverTimer(2 * m_AngleIncrement, m_RotationTime));
                        }
                        else
                        {
                            StartCoroutine(RotateOverTimer(-m_AngleIncrement, m_RotationTime));
                        }
                    }
                    else if (Vector2.Dot(p_Context.ReadValue<Vector2>().normalized, Vector2.right) < 0.2f)
                    {
                        m_CurrentButton = m_CurrentButton - 1;
                        if (m_CurrentButton < 0)
                        {
                            m_CurrentButton = m_MapNames.Count - 1;
                            StartCoroutine(RotateOverTimer(-2 * m_AngleIncrement, m_RotationTime));
                        }
                        else
                        {
                            StartCoroutine(RotateOverTimer(m_AngleIncrement, m_RotationTime));
                        }
                    }
                    DisplayMapBackground(m_CurrentButton);
                    m_MapName.text = m_MapNames[m_CurrentButton];
                    break;
                case MainMenuState.OnOption:
                    m_OptionSelectionFeedback[m_CurrentOptionButton].SetActive(false);
                    m_CurrentOptionButton = m_CurrentOptionButton + 1;
                    if (m_CurrentOptionButton == m_OptionSelectionFeedback.Count)
                    {
                        m_CurrentOptionButton = 0;
                    }
                    m_OptionSelectionFeedback[m_CurrentOptionButton].SetActive(true);
                    break;
            }
        }
    }
    public void SelectInput(InputAction.CallbackContext p_Context)
    {
        if (p_Context.started && !m_InAnimation)
        {
            switch (m_MainMenuState)
            {
                case MainMenuState.OnMainWheel:
                    switch (m_MapNames[m_CurrentButton])
                    {
                        case "Versus fighting":
                            SceneManager.LoadScene("CharacterSelection");
                            break;
                        case "Options":
                            m_MainMenuState = MainMenuState.OnOption;
                            m_OptionsSubMenu.SetActive(true);
                            break;
                        case "Quit game":
                            Application.Quit();
                            break;
                    }
                    break;
                case MainMenuState.OnOption:
                    if (m_CurrentOptionButton == 0)
                    {
                        m_AudioSettingsCanvas.SetActive(true);
                        m_MainMenuInput.DeactivateInput();
                    }
                    else
                    {
                        m_VideoSettingsCanvas.SetActive(true);
                        m_MainMenuInput.DeactivateInput();
                    }
                    break;
            }
        }
    }
    public void ReturnInput(InputAction.CallbackContext p_Context)
    {
        if (p_Context.started && !m_InAnimation)
        {
            switch (m_MainMenuState)
            {
                case MainMenuState.OnMainWheel:
                    Debug.Log("Can't go back further");
                    break;
                case MainMenuState.OnOption:
                    m_MainMenuState = MainMenuState.OnMainWheel;
                    m_OptionsSubMenu.SetActive(false);
                    break;
            }
        }
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
        Debug.Log(l_RotationEffectued);
        m_InAnimation = false;
    }
    #endregion

    private enum MainMenuState
    {
        OnMainWheel,
        OnOption
    }
}

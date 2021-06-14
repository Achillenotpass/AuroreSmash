using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

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
        StartCoroutine(AsyncLoading(m_MapNames[m_CurrentMap], 2.0f));
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

        Quaternion l_RotQuat = m_Buttons.GetComponent<RectTransform>().localRotation;
        Vector3 l_TempRot = Vector3.zero;
        l_TempRot = l_RotQuat.eulerAngles;
        l_TempRot.z = l_TempRot.z + p_RotationAngle;
        m_Buttons.GetComponent<RectTransform>().DOLocalRotate(l_TempRot, p_RotationTime, RotateMode.Fast);

        yield return new WaitForSeconds(p_RotationTime);
        m_InAnimation = false;
    }
    private IEnumerator AsyncLoading(string p_SceneName, float p_MinimumLoadingTime)
    {
        FindObjectOfType<LoadingBackground>().AppearLoadingBackground();
        yield return new WaitForSeconds(p_MinimumLoadingTime);
        Debug.Log("Start loading");
        AsyncOperation l_Scene = SceneManager.LoadSceneAsync(p_SceneName, LoadSceneMode.Single);
        l_Scene.allowSceneActivation = false;
        while (l_Scene.progress < 0.9f)
        {
            Debug.Log(l_Scene.progress);
            yield return null;
        }
        Debug.Log("Done");
        l_Scene.allowSceneActivation = true;
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private Slider m_LoadingBar = null;
    private AsyncOperation m_LoadingProgress = null;
    public static string s_SceneName = string.Empty;
    #endregion
    #region Awake/Start/Update
    private void Start()
    {
        m_LoadingProgress = SceneManager.LoadSceneAsync(s_SceneName);
    }
    private void Update()
    {
        m_LoadingBar.value = m_LoadingProgress.progress;
    }
    #endregion
}

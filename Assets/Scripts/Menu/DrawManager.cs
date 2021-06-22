using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DrawManager : MonoBehaviour
{
    #region Variables
    private bool m_InAnimation = false;
    [SerializeField]
    private Image m_Player1Image = null;
    [SerializeField]
    private Image m_PLayer2Image = null;
    [SerializeField]
    private List<UIObjectList> m_Objects = null;
    private int m_CurrentDisplay = 0;
    #endregion
    #region Awake/Start/Update
    private void Start()
    {
        m_Player1Image.sprite = UsersManager.m_WinnerCharacter.m_PlayedCharacter.VictoryScreenDatas.m_WinnerSprite;
        m_PLayer2Image.sprite = UsersManager.m_LoserCharacter.m_PlayedCharacter.VictoryScreenDatas.m_WinnerSprite;

        for (int i = 0; i < UsersManager.m_WinnerCharacter.m_RemainingLives; i++)
        {
            if (UsersManager.m_WinnerCharacter.m_PlayerIndex == 1)
            {
                m_Player1Image.sprite = UsersManager.m_WinnerCharacter.m_PlayedCharacter.VictoryScreenDatas.m_WinnerSprite;
            }
            else
            {
                m_Player1Image.sprite = UsersManager.m_WinnerCharacter.m_PlayedCharacter.VictoryScreenDatas.m_WinnerSprite;
            }
        }
        for (int i = 0; i < UsersManager.m_LoserCharacter.m_RemainingLives; i++)
        {
            if (UsersManager.m_WinnerCharacter.m_PlayerIndex == 2)
            {
                m_PLayer2Image.sprite = UsersManager.m_LoserCharacter.m_PlayedCharacter.VictoryScreenDatas.m_WinnerSprite;
            }
            else
            {
                m_Player1Image.sprite = UsersManager.m_LoserCharacter.m_PlayedCharacter.VictoryScreenDatas.m_WinnerSprite;
            }
        }
    }
    #endregion
    #region Inputs
    public void SelectInput(InputAction.CallbackContext p_Context)
    {
        if (p_Context.started && !m_InAnimation)
        {
            if (m_CurrentDisplay == m_Objects.Count)
            {
                StartCoroutine(AsyncLoading("CharacterSelection", 1.0f));
            }
            else
            {
                foreach (GameObject l_Object in m_Objects[m_CurrentDisplay].m_Objects)
                {
                    l_Object.SetActive(true);
                }
                m_CurrentDisplay = m_CurrentDisplay + 1;
            }
        }
    }
    public void ReturnInput(InputAction.CallbackContext p_Context)
    {
        if (p_Context.started && !m_InAnimation)
        {
            if (m_CurrentDisplay != 0)
            {
                m_CurrentDisplay = m_CurrentDisplay - 1;
                foreach (GameObject l_Object in m_Objects[m_CurrentDisplay].m_Objects)
                {
                    l_Object.SetActive(false);
                }
            }
        }
    }
    #endregion
    #region Functions
    private IEnumerator AsyncLoading(string p_SceneName, float p_MinimumLoadingTime)
    {
        m_InAnimation = true;

        FindObjectOfType<LoadingBackground>().AppearLoadingBackground();
        yield return new WaitForSeconds(p_MinimumLoadingTime);
        AsyncOperation l_Scene = SceneManager.LoadSceneAsync(p_SceneName, LoadSceneMode.Single);
        l_Scene.allowSceneActivation = false;
        while (l_Scene.progress < 0.9f)
        {
            Debug.Log(l_Scene.progress);
            yield return null;
        }
        l_Scene.allowSceneActivation = true;

        m_InAnimation = false;
    }
    #endregion
    [System.Serializable]
    private class UIObjectList
    {
        public List<GameObject> m_Objects = null;
    }
}

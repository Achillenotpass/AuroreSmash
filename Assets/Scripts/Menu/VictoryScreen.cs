using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private Image m_WinnerImage = null;
    [SerializeField]
    private List<Image> m_Player1Lives = null;
    [SerializeField]
    private Image m_LoserImage = null;
    [SerializeField]
    private List<Image> m_Player2Lives = null;
    [SerializeField]
    private List<UIObjectList> m_Objects = null;
    private int m_CurrentDisplay = 0;
    private bool m_InAnimation = false;
    #endregion

    #region Awake/Start/Update
    private void Start()
    {
        m_WinnerImage.sprite = UsersManager.m_WinnerCharacter.m_PlayedCharacter.VictoryScreenDatas.m_WinnerSprite;
        m_LoserImage.sprite = UsersManager.m_LoserCharacter.m_PlayedCharacter.VictoryScreenDatas.m_LoserSprite;

        for (int i = 0; i < UsersManager.m_WinnerCharacter.m_RemainingLives; i++)
        {
            if(UsersManager.m_WinnerCharacter.m_PlayerIndex == 1)
            {
                m_Player1Lives[i].sprite = UsersManager.m_WinnerCharacter.m_PlayedCharacter.VictoryScreenDatas.m_FaceSprite;
                m_Player1Lives[i].gameObject.SetActive(true);
            }
            else
            {
                m_Player2Lives[i].sprite = UsersManager.m_WinnerCharacter.m_PlayedCharacter.VictoryScreenDatas.m_FaceSprite;
                m_Player2Lives[i].gameObject.SetActive(true);
            }
        }
        for (int i = 0; i < UsersManager.m_LoserCharacter.m_RemainingLives; i++)
        {
            if (UsersManager.m_WinnerCharacter.m_PlayerIndex == 2)
            {
                m_Player2Lives[i].sprite = UsersManager.m_LoserCharacter.m_PlayedCharacter.VictoryScreenDatas.m_FaceSprite;
                m_Player2Lives[i].gameObject.SetActive(true);
            }
            else
            {
                m_Player1Lives[i].sprite = UsersManager.m_LoserCharacter.m_PlayedCharacter.VictoryScreenDatas.m_FaceSprite;
                m_Player1Lives[i].gameObject.SetActive(true);
            }
        }
    }
    #endregion

    #region Inputs
    public void SelectInput(InputAction.CallbackContext p_Context)
    {
        if (p_Context.started)
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
        if (p_Context.started)
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

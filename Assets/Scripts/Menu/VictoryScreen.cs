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
    private List<Image> m_WinnerLives = null;
    [SerializeField]
    private Image m_LoserImage = null;
    [SerializeField]
    private List<Image> m_LoserLives = null;
    [SerializeField]
    private List<UIObjectList> m_Objects = null;
    private int m_CurrentDisplay = 0;
    #endregion

    #region Awake/Start/Update
    private void Start()
    {
        m_WinnerImage.sprite = UsersManager.m_WinnerCharacter.m_PlayedCharacter.VictoryScreenDatas.m_WinnerSprite;
        m_LoserImage.sprite = UsersManager.m_LoserCharacter.m_PlayedCharacter.VictoryScreenDatas.m_LoserSprite;

        for (int i = 0; i < UsersManager.m_WinnerCharacter.m_RemainingLives; i++)
        {
            m_WinnerLives[i].sprite = UsersManager.m_WinnerCharacter.m_PlayedCharacter.VictoryScreenDatas.m_FaceSprite;
            m_WinnerLives[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < UsersManager.m_LoserCharacter.m_RemainingLives; i++)
        {
            m_LoserLives[i].sprite = UsersManager.m_LoserCharacter.m_PlayedCharacter.VictoryScreenDatas.m_FaceSprite;
            m_LoserLives[i].gameObject.SetActive(true);
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
                SceneManager.LoadScene("CharacterSelection");
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

    [System.Serializable]
    private class UIObjectList
    {
        public List<GameObject> m_Objects = null;
    }
}

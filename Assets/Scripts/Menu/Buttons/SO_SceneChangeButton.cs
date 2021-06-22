using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "NewButton", menuName = "ScriptableObjects/Menus/Buttons/NewSceneChangeButton")]
public class SO_SceneChangeButton : SO_Button
{
    #region Variables
    [SerializeField]
    private string m_SceneName = string.Empty;
    #endregion

    #region Functions
    public void PressButton()
    {
        SceneManager.LoadScene(m_SceneName);
    }
    #endregion
}

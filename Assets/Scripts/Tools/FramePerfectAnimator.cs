using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramePerfectAnimator : MonoBehaviour, IUpdateUser
{
    #region UpdateMangaer
    [SerializeField]
    private SO_UpdateLayerSettings m_UpdateSettings = null;
    private void OnEnable()
    {
        m_UpdateSettings.Bind(this);
    }
    private void OnDisable()
    {
        m_UpdateSettings.Unbind(this);
    }
    #endregion

    #region Variables
    [SerializeField]
    private SpriteRenderer m_SpriteRenderer = null;
    [SerializeField]
    private string m_StartingAnimation = string.Empty;
    [SerializeField]
    private List<Animation> m_Animations = new List<Animation>();
    private Animation m_CurrentAnimation = null;
    private int m_CurrentFrameCount = 0;
    private Sprite m_CurrentSprite = null;
    #endregion

    #region Update and stuff
    private void Awake()
    {
        ChangeAnimationTo(m_StartingAnimation);
        //R�cup�rer le nombre global de chaque frame de l'animation
        m_CurrentAnimation.m_Animation.SetGlobalFramePosition();
    }
    public void CustomUpdate(float p_DeltaTime)
    {
        //V�rifier s'il existe une frame dont le nombre global de frame est �gal � la frame actuelle
        //Si c'est le cas, r�cup�rer le sprite
        CheckIfNewSprite(m_CurrentAnimation);
        //et l'afficher
        ChangeActiveSprite(m_CurrentSprite);

        //V�rifier si on a atteint la derni�re frame de l'animation
        if (m_CurrentFrameCount >= m_CurrentAnimation.m_Animation.EndFrame)
        {
            m_CurrentFrameCount = 0;
        }
        else
        {
            //Augmenter le nombre actuel de frames
            m_CurrentFrameCount = m_CurrentFrameCount + 1;
        }
    }
    #endregion

    #region Functions
    private void CheckIfNewSprite(Animation p_AnimationToCheck)
    {
        List<AnimationFrame> l_AnimationFrames = p_AnimationToCheck.m_Animation.AnimationFrames;
        foreach (AnimationFrame l_Frame in l_AnimationFrames)
        {
            if (l_Frame.m_GlobalFramePosition == m_CurrentFrameCount)
            {
                m_CurrentSprite = l_Frame.m_FrameSprite;
            }
        }
    }
    private void ChangeActiveSprite(Sprite p_Sprite)
    {
        m_SpriteRenderer.sprite = p_Sprite;
    }
    public void ChangeAnimationTo(string p_AnimationName)
    {
        foreach (Animation l_Animation in m_Animations)
        {
            if (l_Animation.m_AnimationName == p_AnimationName)
            {
                m_CurrentAnimation = l_Animation;
                m_CurrentFrameCount = 0;

                //R�cup�rer le nombre global de chaque frame de l'animation
                m_CurrentAnimation.m_Animation.SetGlobalFramePosition();
            }
        }
    }
    #endregion
}

[System.Serializable]
public class Animation
{
    public string m_AnimationName = string.Empty;
    public SO_Animation m_Animation = null;
}

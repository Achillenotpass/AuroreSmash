using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCreator : MonoBehaviour
{
    [SerializeField]
    private int m_CurrentFrame = 0;
    public int CurrentFrame { get { return m_CurrentFrame; } set { m_CurrentFrame = value; } }
    [SerializeField]
    private SO_Attack m_Attack = null;
    [SerializeField]
    private SO_Animation m_Animation = null;

    private SpriteRenderer m_SpriteRenderer = null;


    public void GoToNextFrame()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        if (m_Attack == null || m_Animation == null)
        {
            Debug.Log("Not all parameters are set correctly");
            m_CurrentFrame = 0;
        }
        else
        {
            m_Animation.SetGlobalFramePosition();

            if (m_CurrentFrame >= m_Animation.EndFrame)
            {
                m_CurrentFrame = 0;
            }
            else
            {
                m_CurrentFrame = m_CurrentFrame + 1;
            }

            CheckNewSprite();
        }
    }
    public void GoToPreviousFrame()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        if (m_Attack == null || m_Animation == null)
        {
            Debug.Log("Not all parameters are set correctly");
            m_CurrentFrame = 1;
        }
        else
        {
            m_Animation.SetGlobalFramePosition();

            if (m_CurrentFrame == 0)
            {
                m_CurrentFrame = m_Animation.EndFrame;
            }
            else
            {
                m_CurrentFrame = m_CurrentFrame - 1;
            }
        }

        CheckNewSprite();
    }

    private void CheckNewSprite()
    {
        List<AnimationFrame> l_AnimationFrames = m_Animation.AnimationFrames;
        foreach (AnimationFrame l_Frame in l_AnimationFrames)
        {
            if (l_Frame.m_GlobalFramePosition == m_CurrentFrame)
            {
                m_SpriteRenderer.sprite = l_Frame.m_FrameSprite;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        if (m_Attack != null && m_Animation != null)
        {
            foreach (SO_Hit l_Hit in m_Attack.Hits)
            {
                foreach (SO_HitBox l_HitBox in l_Hit.HitBoxes)
                {
                    if (m_CurrentFrame + 1 > l_HitBox.BeforeLag && m_CurrentFrame + 1 <= (l_HitBox.BeforeLag + l_HitBox.Duration))
                    {
                        Vector3 l_HitBoxPosition = Vector3.zero;
                        l_HitBoxPosition.y = transform.position.y + l_HitBox.RelativePosition.y;
                        l_HitBoxPosition.x = transform.position.x + (l_HitBox.RelativePosition.x);
                        l_HitBoxPosition.z = transform.position.z;
                        switch (l_HitBox.HitBoxType)
                        {
                            case EHitBOxType.Square:
                                Gizmos.DrawCube(l_HitBoxPosition, l_HitBox.Size * 2.0f);
                                break;
                            case EHitBOxType.Sphere:
                                Gizmos.DrawSphere(l_HitBoxPosition, l_HitBox.Radius);
                                break;
                        }
                    }
                }
            }
        }
    }
}

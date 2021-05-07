using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Grab : MonoBehaviour, IUpdateUser
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
    private PlayerInfos m_PlayerInfos = null;
    private CharacterInfos m_CharacterInfos = null;

    private CharacterInfos m_Target = null;

    //Hitbox
    [SerializeField]
    private Vector3 m_HitboxRelativePosition = Vector3.zero;
    [SerializeField]
    private float m_HitboxRadius = 0.0f;

    private GrabState m_CurrentGrabState = GrabState.NotGrabbing;
    private int m_CurrentFrameCount = 0;
    [SerializeField]
    private int m_PreLag = 10;
    [SerializeField]
    private int m_GrabDuration = 10;
    [SerializeField]
    private int m_GrabHoldDuration = 120;
    [SerializeField]
    private int m_ThrowDuration = 10;
    [SerializeField]
    private int m_FailedLag = 10;

    //Events
    [SerializeField]
    private UnityEvent m_StartGrabEvent = null;
    [SerializeField]
    private UnityEvent m_GrabEnemy = null;
    [SerializeField]
    private UnityEvent m_ThrowEnemy = null;
    [SerializeField]
    private UnityEvent m_EndGrab = null;

    #endregion

    #region Update/Awake/Start
    private void Awake()
    {
        m_PlayerInfos = GetComponent<PlayerInfos>();
        m_CharacterInfos = GetComponent<CharacterInfos>();
    }
    public void CustomUpdate(float p_DeltaTime)
    {
        if (m_CharacterInfos.CurrentCharacterState == CharacterState.Grabbing)
        {
            if (m_Target == null)
            {
                switch (m_CurrentGrabState)
                {
                    case GrabState.NotGrabbing:
                        break;
                    case GrabState.PreLag:
                        if (m_CurrentFrameCount >= m_PreLag)
                        {
                            m_CurrentGrabState = GrabState.Grabbing;
                            m_CurrentFrameCount = 0;
                        }
                        break;
                    case GrabState.Grabbing:
                        TryGrab();

                        if (m_CurrentFrameCount >= m_GrabDuration)
                        {
                            m_CurrentGrabState = GrabState.FailedLag;
                            m_CurrentFrameCount = 0;
                        }
                        break;
                    case GrabState.PostLag:
                        if (m_CurrentFrameCount >= m_ThrowDuration)
                        {
                            m_CurrentGrabState = GrabState.NotGrabbing;
                            m_CharacterInfos.CurrentCharacterState = CharacterState.Idle;
                            m_CurrentFrameCount = 0;

                            m_EndGrab.Invoke();
                        }
                        break;
                    case GrabState.FailedLag:
                        if (m_CurrentFrameCount >= m_FailedLag)
                        {
                            m_CurrentGrabState = GrabState.NotGrabbing;
                            m_CharacterInfos.CurrentCharacterState = CharacterState.Idle;
                            m_CurrentFrameCount = 0;

                            m_EndGrab.Invoke();
                        }
                        break;
                }
            }
            else
            {
                if (m_CurrentFrameCount >= m_GrabHoldDuration)
                {
                    m_Target.CurrentCharacterState = CharacterState.Idle;
                    m_Target = null;
                    m_CurrentGrabState = GrabState.PostLag;
                    m_CurrentFrameCount = 0;
                }
            }

            m_CurrentFrameCount = m_CurrentFrameCount + 1;
        }
    }
    #endregion

    #region Inputs
    public void GrabInput(InputAction.CallbackContext p_Context)
    {
        if (p_Context.started)
        {
            if (m_CharacterInfos.CurrentCharacterState == CharacterState.Idle
                || m_CharacterInfos.CurrentCharacterState == CharacterState.Moving)
            {
                m_CurrentFrameCount = 0;

                m_CharacterInfos.CurrentCharacterState = CharacterState.Grabbing;
                m_CurrentGrabState = GrabState.PreLag;

                m_StartGrabEvent.Invoke();
            }
        }
    }
    public void ThrowInput(InputAction.CallbackContext p_Context)
    {
        if (m_Target != null)
        {
            Vector2 l_ThrowDirection = p_Context.ReadValue<Vector2>();
            if (l_ThrowDirection.magnitude >= 0.2f)
            {
                SO_HitBox l_HitBox = new SO_HitBox();

                m_Target.GetComponent<Health>().TakeDamages(l_HitBox);
                m_Target = null;
                m_CurrentGrabState = GrabState.PostLag;
                m_ThrowEnemy.Invoke();
                m_CurrentFrameCount = 0;
            }
        }
    }
    #endregion

    #region Functions
    private void TryGrab()
    {
        Collider[] l_HitObjects =  Physics.OverlapSphere(transform.position + m_HitboxRelativePosition, m_HitboxRadius, m_PlayerInfos.AttackableLayers);
        if (l_HitObjects != null)
        {
            foreach (Collider l_Collider in l_HitObjects)
            {
                CharacterInfos l_HitCharacter = l_Collider.GetComponent<CharacterInfos>();
                if (l_HitCharacter != null)
                {
                    if (l_HitCharacter != m_CharacterInfos)
                    {
                        l_HitCharacter.CurrentCharacterState = CharacterState.Grabbed;

                        m_Target = l_HitCharacter;

                        m_GrabEnemy.Invoke();
                    }
                }
            }
        }
    }
    #endregion

    private enum GrabState
    {
        NotGrabbing,
        PreLag,
        Grabbing,
        PostLag,
        FailedLag,
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour, IUpdateUser
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
    private SO_Attack m_Jab = null;
    private EAttackState m_CurrentAttackState = EAttackState.Nothing;
    public int m_CurrentFrameCount = 0;
    private float m_CurrentTimeCount = 0.0f;
    private PlayerInfos m_PlayerInfos = null;
    private int m_MaxFrameCount = 0;
    #endregion

    #region Input
    public void AttackInputPressed(InputAction.CallbackContext p_Context)
    {
        //Si on appuie sur la touche d'attaque et qu'on n'est pas déjà en train d'attaquer
        if (p_Context.performed && m_CurrentAttackState == EAttackState.Nothing)
        {
            Debug.Log("StartAttack");
            //On passe en état d'attaque
            m_CurrentAttackState = EAttackState.Jab;
            SetMaxAttackDuration(m_Jab);
        }
    }
    #endregion

    #region Awake/Start/update
    private void Awake()
    {
        m_PlayerInfos = GetComponent<PlayerInfos>();
    }
    public void CustomUpdate(float p_DeltaTime)
    {
        //Si on est en train d'attaquer on augmente le timer en secondes depuis le déubt de l'attaque
        if (m_CurrentAttackState != EAttackState.Nothing)
        {
            m_CurrentTimeCount = m_CurrentTimeCount + p_DeltaTime;
            //On multiplie par 60 pour avoir le nombre de frames
            m_CurrentFrameCount = Mathf.RoundToInt(m_CurrentTimeCount * 60.0f);
            //Si on dépasse le nombre de frame maximal de l'attaque (lag compris)
            if (m_CurrentFrameCount > m_MaxFrameCount)
            {
                //On finit l'attaque
                m_CurrentAttackState = EAttackState.Nothing;
            }
        }
        else
        {
            //Et si on et pas en train d'attaquer on remet le timer à 0
            m_CurrentTimeCount = 0.0f;
            //Ainsi que le nombre de frames
            m_CurrentFrameCount = 0;
        }

        switch (m_CurrentAttackState)
        {
            case EAttackState.Nothing:
                break;
            case EAttackState.Jab:
                CheckAttackHitBoxesFrames(m_Jab);
                break;
        }
    }
    #endregion

    #region Functions
    //CALCUL DE LA DUREE MAX DE L'ATTAQUE
    public void SetMaxAttackDuration(SO_Attack p_AttackStats)
    {
        foreach (SO_Hit l_Hit in p_AttackStats.Hit)
        {
            foreach (SO_HitBox l_HitBox in l_Hit.HitBoxes)
            {
                //On compare quelle hitbox a la durée (pre-lag, durée, post-lag) la plus élévée
                m_MaxFrameCount = Mathf.Max(m_MaxFrameCount, l_HitBox.BeforeLag + l_HitBox.Duration + p_AttackStats.AfterLag);
            }
        }
    }
    public void CheckAttackHitBoxesFrames(SO_Attack p_AttackStats)
    {
        foreach (SO_Hit l_Hit in p_AttackStats.Hit)
        {
            foreach (SO_HitBox l_HitBox in l_Hit.HitBoxes)
            {
                //On regarde pour chaque Hitbox si elle est censée s'activer à la frame où on est actuellement
                if (m_CurrentFrameCount > l_HitBox.BeforeLag && m_CurrentFrameCount <= (l_HitBox.BeforeLag + l_HitBox.Duration))
                {
                    //Si oui, on l'active
                    CheckEnemiesInRange(l_HitBox);
                }
                else if(m_CurrentFrameCount >= (l_HitBox.BeforeLag + l_HitBox.Duration))
                {
                    //POST-LAG
                }
            }
        }
    }
    public void CheckEnemiesInRange(SO_HitBox p_HitBox)
    {
        Collider[] l_HitObjects = null;
        //On récupère tous les objets qui ont un collider qu'on peut attaquer dans la hitbox
        switch (p_HitBox.HitBoxType)
        {
            case EHitBOxType.Square:
                Physics.OverlapBoxNonAlloc(transform.position + p_HitBox.RelativePosition, p_HitBox.Size, l_HitObjects);
                break;
            case EHitBOxType.Sphere:
                Physics.OverlapSphereNonAlloc(transform.position + Vector3.right, p_HitBox.Radius, l_HitObjects, m_PlayerInfos.AttackableLayers);
                break;
        }
        if (l_HitObjects != null)
        {
            //Si on a touché des objets, on leur applique des dégâts
            ApplyDamages(l_HitObjects, p_HitBox);
        }
    }
    public void ApplyDamages(Collider[] p_HitPlayer, SO_HitBox p_HitBox)
    {
        //Pour chaque collider trouvé précédemment
        foreach (Collider l_HitObject in p_HitPlayer)
        {
            //On vérifie si c'est un  joueur
            Health l_HitObjectHealth = l_HitObject.GetComponentInChildren<Health>();
            //On inflige les dégâts aux joueurs touchés
            l_HitObjectHealth.TakeDamages(p_HitBox.Damages);
        }
    }
    #endregion
}

public enum EAttackState
{
    Nothing,
    Jab,
    SideTilt,
    UpTilt,
    DownTilt,
    NeutralAir,
    ForwardAir,
    BackAir,
    DownAir,
}

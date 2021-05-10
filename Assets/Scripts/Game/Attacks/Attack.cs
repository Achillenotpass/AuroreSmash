using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;

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
    private int m_CurrentFrameCount = 0;
    private PlayerInfos m_PlayerInfos = null;
    private CharacterInfos m_CharacterInfos = null;
    private int m_MaxFrameCount = 0;
    private Dictionary<SO_Hit, List<Health>> m_PlayersHit = new Dictionary<SO_Hit, List<Health>>();
    private SO_Attack m_CurrentAttack = null;

    //ATTAQUES/INPUTS
    private SO_Attack m_LastAttack = null;
    private Vector2 m_AimDirection = Vector2.zero;
    public bool m_IsAerial = false;
    private CharacterMovement m_PlayerMovements = null;
    private SO_Attack m_ComboBuffer = null;
    [SerializeField]
    private float m_JoystickDeadZone = 0.2f;
    [SerializeField]
    private Attacks m_Attacks = new Attacks();
    #endregion

    #region Events
    [SerializeField]
    private AttackEvents m_AttackEvents = new AttackEvents();
    #endregion

    #region Input
    public void AttackInputPressed(InputAction.CallbackContext p_Context)
    {
        if (m_CharacterInfos.CurrentCharacterState == CharacterState.Idle
            || m_CharacterInfos.CurrentCharacterState == CharacterState.Moving
            || m_CharacterInfos.CurrentCharacterState == CharacterState.Attacking)
        {
            //Le joueur doit relâcher la touche d'attaque avant de pouvoir s'en servir de nouveau
            if (p_Context.canceled)
            {
                m_LastAttack = null;
            }
            else if (p_Context.started)
            {
                CheckAttackInput(m_AimDirection.normalized);
            }
        }
    }
    public void RightJoystickUsed(InputAction.CallbackContext p_Context)
    {
        if (m_CharacterInfos.CurrentCharacterState == CharacterState.Idle
            || m_CharacterInfos.CurrentCharacterState == CharacterState.Moving
            || m_CharacterInfos.CurrentCharacterState == CharacterState.Attacking)
        {
            if (p_Context.ReadValue<Vector2>().magnitude >= m_JoystickDeadZone)
            {
                CheckAttackInput(p_Context.ReadValue<Vector2>().normalized);
            }
            else
            {
                m_LastAttack = null;
            }
        }
    }
    public void LeftJoystickUsed(InputAction.CallbackContext p_Context)
    {
        m_AimDirection = p_Context.ReadValue<Vector2>().normalized;
    }
    #endregion

    #region Awake/Start/update
    private void Awake()
    {
        m_PlayerInfos = GetComponent<PlayerInfos>();
        m_CharacterInfos = GetComponent<CharacterInfos>();
        m_PlayerMovements = GetComponent<CharacterMovement>();
    }
    public void CustomUpdate(float p_DeltaTime)
    {
        //Si on est en train d'attaquer on augmente le timer en secondes depuis le début de l'attaque
        if (m_CurrentAttack != null)
        {
            AttackMovePlayer(m_CurrentAttack);
            CheckAttackFrames(m_CurrentAttack);
            m_CurrentFrameCount = m_CurrentFrameCount + 1;

            //On modifie la vitesse du personnage
            m_PlayerMovements.EditableCharacterSpeed = m_CurrentAttack.PlayerInfluenceOnSpeed.Evaluate(m_CurrentFrameCount);
            //On empêche le joueur de se retourner pendant l'attaque
            m_CharacterInfos.CurrentCharacterState = CharacterState.Attacking;
            //Si on dépasse le nombre de frame maximal de l'attaque (lag compris)
            if (m_CurrentFrameCount >= m_MaxFrameCount)
            {
                //On finit l'attaque
                InterruptAttack();
            }
        }
    }
    #endregion

    #region Functions
    //INTERRUPTION DE L'ATTAQUE
    public void InterruptAttack()
    {
        m_MaxFrameCount = 0;
        m_CurrentFrameCount = 0;
        if (m_ComboBuffer != null)
        {
            //On passe dan un état où on n'attaque plus
            m_CurrentAttack = m_ComboBuffer;
            m_ComboBuffer = null;
            //On réinitialise la liste des joueurs touchés par l'attaque
            m_PlayersHit.Clear();

            SetMaxAttackDuration(m_CurrentAttack);
        }
        else
        {
            //On passe dan un état où on n'attaque plus
            m_CurrentAttack = null;
            //On réinitialise la liste des joueurs touchés par l'attaque
            m_PlayersHit.Clear();
            //On envoie les feedbacks
            m_AttackEvents.m_InterruptAttack.Invoke();


            //On permet joueur de se retourner pendant l'attaque
            m_CharacterInfos.CurrentCharacterState = CharacterState.Idle;
            //On rend au joueur sa vitesse normale
            m_PlayerMovements.EditableCharacterSpeed = 1.0f;
        }
        m_PlayerMovements.PlayerExternalDirection = Vector3.zero;
    }
    public void CheckAttackInput(Vector2 p_JoyStickInput)
    {
        //On calcule l'angle de la direction du joystick par rapport à un vecteur 1,0,0 (vers la droite)
        float l_Angle = Vector2.Angle(new Vector2(1.0f, 0.0f), p_JoyStickInput.normalized);
        l_Angle = l_Angle * Mathf.Sign(p_JoyStickInput.y);
        if (m_CurrentAttack != null)
        {
            if (m_CurrentAttack.Combo != null && m_LastAttack == null)
            {
                m_ComboBuffer = m_CurrentAttack.Combo;
            }
        }
        else if (m_LastAttack == null)
        {
            if (m_PlayerMovements.IsGrounded)
            {
                if (p_JoyStickInput.magnitude >= m_JoystickDeadZone)
                {
                    //Vers la gauche
                    if (l_Angle >= 135.0f)
                    {//Penser à retourner le joueur si nécessaire
                        TurnLeft(true);
                        m_CurrentAttack = m_Attacks.m_SideTilt;
                        SetMaxAttackDuration(m_Attacks.m_SideTilt);
                        //On envoie les feedbacks
                        m_AttackEvents.m_StartSideTilt.Invoke();
                    }
                    //Vers le haut
                    else if (l_Angle >= 45.0f)
                    {
                        m_CurrentAttack = m_Attacks.m_UpTilt;
                        SetMaxAttackDuration(m_Attacks.m_UpTilt);
                        //On envoie les feedbacks
                        m_AttackEvents.m_StartUpTilt.Invoke();
                    }
                    //Vers la droite
                    else if (l_Angle >= -45.0f)
                    {//Penser à retourner le joueur si nécessaire
                        TurnLeft(false);
                        m_CurrentAttack = m_Attacks.m_SideTilt;
                        SetMaxAttackDuration(m_Attacks.m_SideTilt);
                        //On envoie les feedbacks
                        m_AttackEvents.m_StartSideTilt.Invoke();
                    }
                    //Vers le bas
                    else if (l_Angle >= -135.0f)
                    {
                        m_CurrentAttack = m_Attacks.m_DownTilt;
                        SetMaxAttackDuration(m_Attacks.m_DownTilt);
                        //On envoie les feedbacks
                        m_AttackEvents.m_StartDownTilt.Invoke();
                    }
                    //Vers la gauche
                    else
                    {//Penser à retourner le joueur si nécessaire
                        TurnLeft(true);
                        m_CurrentAttack = m_Attacks.m_SideTilt;
                        SetMaxAttackDuration(m_Attacks.m_SideTilt);
                        //On envoie les feedbacks
                        m_AttackEvents.m_StartSideTilt.Invoke();
                    }
                }
                else
                {
                    m_CurrentAttack = m_Attacks.m_Jab;
                    SetMaxAttackDuration(m_Attacks.m_Jab);
                    //On envoie les feedbacks
                    m_AttackEvents.m_StartJab.Invoke();
                }

            }
            //AERIALS
            else
            {
                if (p_JoyStickInput.magnitude >= m_JoystickDeadZone)
                {
                    //Vers la gauche
                    if (l_Angle >= 135.0f)
                    {
                        if (m_PlayerMovements.CharacterOrientation > 0)
                        {
                            m_CurrentAttack = m_Attacks.m_BackAir;
                            SetMaxAttackDuration(m_Attacks.m_BackAir);
                            //On envoie les feedbacks
                            m_AttackEvents.m_StartBackAir.Invoke();
                        }
                        else
                        {
                            m_CurrentAttack = m_Attacks.m_ForwardAir;
                            SetMaxAttackDuration(m_Attacks.m_ForwardAir);
                            //On envoie les feedbacks
                            m_AttackEvents.m_StartForwardAir.Invoke();
                        }
                    }
                    //Vers le haut
                    else if (l_Angle >= 45.0f)
                    {
                        m_CurrentAttack = m_Attacks.m_UpAir;
                        SetMaxAttackDuration(m_Attacks.m_UpAir);
                        //On envoie les feedbacks
                        m_AttackEvents.m_StartUpAir.Invoke();
                    }
                    //Vers la droite
                    else if (l_Angle >= -45.0f)
                    {
                        if (m_PlayerMovements.CharacterOrientation > 0)
                        {
                            m_CurrentAttack = m_Attacks.m_ForwardAir;
                            SetMaxAttackDuration(m_Attacks.m_ForwardAir);
                            //On envoie les feedbacks
                            m_AttackEvents.m_StartForwardAir.Invoke();
                        }
                        else
                        {
                            m_CurrentAttack = m_Attacks.m_BackAir;
                            SetMaxAttackDuration(m_Attacks.m_BackAir);
                            //On envoie les feedbacks
                            m_AttackEvents.m_StartBackAir.Invoke();
                        }
                    }
                    //Vers le bas
                    else if (l_Angle >= -135.0f)
                    {
                        m_CurrentAttack = m_Attacks.m_DownAir;
                        SetMaxAttackDuration(m_Attacks.m_DownAir);
                        //On envoie les feedbacks
                        m_AttackEvents.m_StartDownAir.Invoke();
                    }
                    //Vers la gauche
                    else
                    {
                        if (m_PlayerMovements.CharacterOrientation > 0)
                        {
                            m_CurrentAttack = m_Attacks.m_BackAir;
                            SetMaxAttackDuration(m_Attacks.m_BackAir);
                            //On envoie les feedbacks
                            m_AttackEvents.m_StartBackAir.Invoke();
                        }
                        else
                        {
                            m_CurrentAttack = m_Attacks.m_ForwardAir;
                            SetMaxAttackDuration(m_Attacks.m_ForwardAir);
                            //On envoie les feedbacks
                            m_AttackEvents.m_StartForwardAir.Invoke();
                        }
                    }
                }
                else
                {
                    m_CurrentAttack = m_Attacks.m_NeutralAir;
                    SetMaxAttackDuration(m_Attacks.m_NeutralAir);
                    //On envoie les feedbacks
                    m_AttackEvents.m_StartNeutralAir.Invoke();
                }


            }
        }

    }
    public void TurnLeft(bool p_Left)
    {
        if (p_Left)
        {
            m_PlayerMovements.PlayerOrientation(-1.0f);
        }
        else
        {
            m_PlayerMovements.PlayerOrientation(1.0f);
        }
    }
    //CALCUL DE LA DUREE MAX DE L'ATTAQUE
    public void SetMaxAttackDuration(SO_Attack p_AttackStats)
    {
        foreach (SO_Hit l_Hit in p_AttackStats.Hits)
        {
            foreach (SO_HitBox l_HitBox in l_Hit.HitBoxes)
            {
                //On compare quelle hitbox a la durée (pre-lag, durée, post-lag) la plus élévée
                m_MaxFrameCount = Mathf.Max(m_MaxFrameCount, l_HitBox.BeforeLag + l_HitBox.Duration + p_AttackStats.AfterLag);
            }
        }
        foreach (SO_Projectile l_ProjectileStat in p_AttackStats.Projectiles)
        {
            //On vérifie aussi si un projectile s'instantie plus tard que le reste
            m_MaxFrameCount = Mathf.Max(m_MaxFrameCount, l_ProjectileStat.InstantiationFrame + p_AttackStats.AfterLag);
        }
    }
    public void CheckAttackFrames(SO_Attack p_AttackStats)
    {
        foreach (SO_Hit l_Hit in p_AttackStats.Hits)
        {
            foreach (SO_HitBox l_HitBox in l_Hit.HitBoxes)
            {
                //On regarde pour chaque Hitbox si elle est censée s'activer à la frame où on est actuellement
                if (m_CurrentFrameCount > l_HitBox.BeforeLag && m_CurrentFrameCount <= (l_HitBox.BeforeLag + l_HitBox.Duration))
                {
                    //Si oui, on l'active
                    CheckEnemiesInRange(l_HitBox, l_Hit);
                }
            }
        }
        //On regarde pour chaque projectile de l'attaque si on est à la frame d'instantiation
        foreach (SO_Projectile l_ProjectileStat in p_AttackStats.Projectiles)
        {
            if (m_CurrentFrameCount == l_ProjectileStat.InstantiationFrame - 1)
            {
                FireProjectile(l_ProjectileStat);
            }
        }
    }
    public void CheckEnemiesInRange(SO_HitBox p_HitBox, SO_Hit p_Hit)
    {
        Collider[] l_HitObjects = null;
        Vector3 l_HitBoxPosition = Vector3.zero;
        l_HitBoxPosition.y = transform.position.y + p_HitBox.RelativePosition.y;
        l_HitBoxPosition.x = transform.position.x + (p_HitBox.RelativePosition.x * Mathf.Sign(m_PlayerMovements.CharacterOrientation));
        l_HitBoxPosition.z = transform.position.z;


        //On récupère tous les objets qui ont un collider qu'on peut attaquer dans la hitbox
        switch (p_HitBox.HitBoxType)
        {
            case EHitBOxType.Square:
                l_HitObjects = Physics.OverlapBox(l_HitBoxPosition, p_HitBox.Size, Quaternion.identity, m_PlayerInfos.AttackableLayers);
                break;
            case EHitBOxType.Sphere:
                l_HitObjects = Physics.OverlapSphere(l_HitBoxPosition, p_HitBox.Radius, m_PlayerInfos.AttackableLayers);
                break;
        }
        if (l_HitObjects != null)
        {//Pour chaque collider trouvé précédemment
            foreach (Collider l_HitObject in l_HitObjects)
            {
                //On vérifie si c'est un  bouclier
                if (l_HitObject.gameObject.tag == "Shield")
                {
                    Health l_HitObjectHealth = l_HitObject.gameObject.GetComponentInParent<Health>();
                    if (l_HitObjectHealth != null)
                    {
                        //On regarde si le coup actuellement en cours a déjà touché quelqu'un
                        if (m_PlayersHit.TryGetValue(p_Hit, out List<Health> l_HitPlayers))
                        {
                            bool l_AlreadyHit = false;
                            //Si oui, on regarde s'il a déjà touché ce joueur
                            for (int i = 0; i < l_HitPlayers.Count; i++)
                            {
                                if (l_HitPlayers[i] == l_HitObjectHealth)
                                {
                                    l_AlreadyHit = true;
                                    break;
                                }
                            }
                            if (!l_AlreadyHit)
                            {
                                //Si le coup n'a pas touché ce joueur on applique les dégâts
                                ApplyDamagesShield(p_Hit, p_HitBox, l_HitObjectHealth);
                            }
                        }
                        //Si le coup n'a touché personne on applique les dégâts
                        else
                        {
                            ApplyDamagesShield(p_Hit, p_HitBox, l_HitObjectHealth);
                        }
                    }
                }
            }


            //Pour chaque collider trouvé précédemment
            foreach (Collider l_HitObject in l_HitObjects)
            {
                //On vérifie si c'est un  joueur
                Health l_HitObjectHealth = l_HitObject.GetComponentInChildren<Health>();
                //SI c'est un joueur
                if (l_HitObjectHealth != null)
                {
                    //On regarde si le coup actuellement en cours a déjà touché quelqu'un
                    if (m_PlayersHit.TryGetValue(p_Hit, out List<Health> l_HitPlayers))
                    {
                        bool l_AlreadyHit = false;
                        //Si oui, on regarde s'il a déjà touché ce joueur
                        for (int i = 0; i < l_HitPlayers.Count; i++)
                        {
                            if (l_HitPlayers[i] == l_HitObjectHealth)
                            {
                                l_AlreadyHit = true;
                                break;
                            }
                        }
                        if (!l_AlreadyHit)
                        {
                            //Si le coup n'a pas touché ce joueur on applique les dégâts
                            ApplyDamages(p_Hit, p_HitBox, l_HitObjectHealth);
                        }
                    }
                    //Si le coup n'a touché personne on applique les dégâts
                    else
                    {
                        ApplyDamages(p_Hit, p_HitBox, l_HitObjectHealth);
                    }
                }
            }
        }
    }
    public void ApplyDamages(SO_Hit p_Hit, SO_HitBox p_HitBox, Health p_PlayerHit)
    {
        //Oninflige les dégâts au joueur touché
        p_PlayerHit.TakeDamages(p_HitBox);
        //On ajoute le joueur touché à la liste des joueurs touchés
        AddPlayerToDictionary(p_Hit, p_PlayerHit);
    }
    public void ApplyDamagesShield(SO_Hit p_Hit, SO_HitBox p_HitBox, Health p_PlayerHit)
    {
        //Oninflige les dégâts au joueur touché
        p_PlayerHit.GetComponent<Shield>().TakeShieldDamages(p_HitBox);
        //On ajoute le joueur touché à la liste des joueurs touchés
        AddPlayerToDictionary(p_Hit, p_PlayerHit);
    }
    public void AddPlayerToDictionary(SO_Hit p_Hit, Health p_HitPlayer)
    {
        //S'il existe déjà une liste de joueurs touchés par ce coup, on ajoute le joueur à la liste
        if (m_PlayersHit.TryGetValue(p_Hit, out List<Health> l_HitPlayers))
        {
            l_HitPlayers.Add(p_HitPlayer);
        }
        //Sinon, on crée une liste qu'on associe au coup, puis on ajoute le joueur à la liste
        else
        {
            m_PlayersHit.Add(p_Hit, new List<Health>() { p_HitPlayer });
        }
    }
    //CREATION DE PROJECTILE
    public void FireProjectile(SO_Projectile p_Projectile)
    {
        GameObject m_InstantiatedProjectile = null;
        //On instancie le projectile
        Vector3 l_ProjectilePosition = Vector3.zero;
        l_ProjectilePosition.y = transform.position.y + p_Projectile.RelativeStartPosition.y;
        l_ProjectilePosition.x = transform.position.x + (p_Projectile.RelativeStartPosition.x * m_PlayerMovements.CharacterOrientation);
        m_InstantiatedProjectile = Instantiate(p_Projectile.ProjectilePrefab, l_ProjectilePosition, Quaternion.identity);
        //On le met dans la bonne direction
        if (m_PlayerMovements.CharacterOrientation > 0)
        {
            //Si le joueur regarde à droite, on garde l'angle de base
            m_InstantiatedProjectile.transform.Rotate(Vector3.forward, p_Projectile.ShootAngle);
        }
        else
        {
            //S'il regarde à gauche, on calcule l'angle correspondant (180° - Angle)
            m_InstantiatedProjectile.transform.Rotate(Vector3.forward, 180.0f - p_Projectile.ShootAngle);
        }
        //On lui donne les cibles qu'il peut attaquer
        m_InstantiatedProjectile.GetComponent<Projectile>().AttackableLayer = m_PlayerInfos.AttackableLayers;
        //Et on lui donne ses statistiques
        m_InstantiatedProjectile.GetComponent<Projectile>().ProjectileStats = p_Projectile;
    }
    private void AttackMovePlayer(SO_Attack m_CurrentAttack)
    {
        Vector3 l_MoveDirection = Vector3.zero;
        l_MoveDirection.x = m_CurrentAttack.CharacterXMovement.Evaluate(m_CurrentFrameCount);
        l_MoveDirection.y = m_CurrentAttack.CharacterYMovement.Evaluate(m_CurrentFrameCount);

        m_PlayerMovements.PlayerExternalDirection = l_MoveDirection;
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        if (m_CurrentAttack != null)
        {
            GizmosFunction(m_CurrentAttack);
        }
    }
    public void GizmosFunction(SO_Attack p_Attack)
    {
        Color l_GizmosColor = Color.magenta;
        l_GizmosColor.a = 0.5f;
        Gizmos.color = l_GizmosColor;
        foreach (SO_Hit l_Hit in p_Attack.Hits)
        {
            foreach (SO_HitBox l_HitBox in l_Hit.HitBoxes)
            {
                if (m_CurrentFrameCount > l_HitBox.BeforeLag && m_CurrentFrameCount <= (l_HitBox.BeforeLag + l_HitBox.Duration))
                {
                    Vector3 l_HitBoxPosition = Vector3.zero;
                    l_HitBoxPosition.y = transform.position.y + l_HitBox.RelativePosition.y;
                    l_HitBoxPosition.x = transform.position.x + (l_HitBox.RelativePosition.x * Mathf.Sign(m_PlayerMovements.CharacterOrientation));
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
        foreach (SO_Projectile l_ProjectileStat in p_Attack.Projectiles)
        {
            if (m_CurrentFrameCount == l_ProjectileStat.InstantiationFrame)
            {
                Vector3 l_InstantiationPosition = Vector3.zero;
                l_InstantiationPosition.x = transform.position.x + (l_ProjectileStat.RelativeStartPosition.x * Mathf.Sign(m_PlayerMovements.CharacterOrientation));
                l_InstantiationPosition.y = transform.position.y + l_ProjectileStat.RelativeStartPosition.y;
                Gizmos.DrawSphere(l_InstantiationPosition, 0.5f);
            }
        }
    }
    #endregion
}

#region Classes
[System.Serializable]
public class Attacks
{
    [SerializeField]
    public SO_Attack m_Jab = null;
    [SerializeField]
    public SO_Attack m_SideTilt = null;
    [SerializeField]
    public SO_Attack m_UpTilt = null;
    [SerializeField]
    public SO_Attack m_DownTilt = null;
    [SerializeField]
    public SO_Attack m_NeutralAir = null;
    [SerializeField]
    public SO_Attack m_ForwardAir = null;
    [SerializeField]
    public SO_Attack m_UpAir = null;
    [SerializeField]
    public SO_Attack m_DownAir = null;
    [SerializeField]
    public SO_Attack m_BackAir = null;
}

[System.Serializable]
public class AttackEvents
{
    [SerializeField]
    public UnityEvent m_InterruptAttack = null;
    [SerializeField]
    public UnityEvent m_StartJab = null;
    [SerializeField]
    public UnityEvent m_StartSideTilt = null;
    [SerializeField]
    public UnityEvent m_StartUpTilt = null;
    [SerializeField]
    public UnityEvent m_StartDownTilt = null;
    [SerializeField]
    public UnityEvent m_StartNeutralAir = null;
    [SerializeField]
    public UnityEvent m_StartForwardAir = null;
    [SerializeField]
    public UnityEvent m_StartUpAir = null;
    [SerializeField]
    public UnityEvent m_StartDownAir = null;
    [SerializeField]
    public UnityEvent m_StartBackAir = null;
}
#endregion

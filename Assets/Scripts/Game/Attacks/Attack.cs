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
    private EAttackState m_CurrentAttackState = EAttackState.Nothing;
    private int m_CurrentFrameCount = 0;
    private float m_CurrentTimeCount = 0.0f;
    private PlayerInfos m_PlayerInfos = null;
    private int m_MaxFrameCount = 0;
    private Dictionary<SO_Hit, List<Health>> m_PlayersHit = new Dictionary<SO_Hit, List<Health>>();

    //ATTAQUES/INPUTS
    private EAttackState m_LastAttack = EAttackState.Nothing;
    private Vector2 m_AimDirection = Vector2.zero;
    public bool m_IsAerial = false;
    public int m_PlayerDirection = 1; //1 = Right/-1 = left
    [SerializeField]
    private float m_JoystickDeadZone = 0.2f;
    [SerializeField]
    private SO_Attack m_Jab = null;
    [SerializeField]
    private SO_Attack m_SideTilt = null;
    [SerializeField]
    private SO_Attack m_UpTilt = null;
    [SerializeField]
    private SO_Attack m_DownTilt = null;
    [SerializeField]
    private SO_Attack m_NeutralAir = null;
    [SerializeField]
    private SO_Attack m_ForwardAir = null;
    [SerializeField]
    private SO_Attack m_UpAir = null;
    [SerializeField]
    private SO_Attack m_DownAir = null;
    [SerializeField]
    private SO_Attack m_BackAir = null;
    #endregion

    #region Input
    public void AttackInputPressed(InputAction.CallbackContext p_Context)
    {
        //Le joueur doit rel�cher la touche d'attaque avant de pouvoir s'en servir de nouveau
        if (p_Context.canceled)
        {
            m_LastAttack = EAttackState.Nothing;
        }
        else
        {
            CheckAttackInput(m_AimDirection.normalized);
        }
    }
    public void RightJoystickUsed(InputAction.CallbackContext p_Context)
    {
        if (p_Context.ReadValue<Vector2>().magnitude >= m_JoystickDeadZone)
        {
            CheckAttackInput(p_Context.ReadValue<Vector2>().normalized);
        }
        else
        {
            m_LastAttack = EAttackState.Nothing;
        }
    }
    public void LeftJoystickUsed(InputAction.CallbackContext p_Context)
    {
        m_AimDirection = p_Context.ReadValue<Vector2>();
    }
    #endregion

    #region Awake/Start/update
    private void Awake()
    {
        m_PlayerInfos = GetComponent<PlayerInfos>();
    }
    public void CustomUpdate(float p_DeltaTime)
    {
        //Si on est en train d'attaquer on augmente le timer en secondes depuis le d�ubt de l'attaque
        if (m_CurrentAttackState != EAttackState.Nothing)
        {
            m_CurrentTimeCount = m_CurrentTimeCount + p_DeltaTime;
            //On multiplie par 60 pour avoir le nombre de frames
            m_CurrentFrameCount = Mathf.RoundToInt(m_CurrentTimeCount * 60.0f);
            //Si on d�passe le nombre de frame maximal de l'attaque (lag compris)
            if (m_CurrentFrameCount >= m_MaxFrameCount)
            {
                //On finit l'attaque
                InterruptAttack();
            }
        }
        else
        {
            //Et si on et pas en train d'attaquer on remet le timer � 0
            m_CurrentTimeCount = 0.0f;
            //Ainsi que le nombre de frames
            m_CurrentFrameCount = 0;
        }

        Debug.Log(m_CurrentAttackState);
        switch (m_CurrentAttackState)
        {
            case EAttackState.Nothing:
                break;
            case EAttackState.Jab:
                CheckAttackFrames(m_Jab);
                break;
            case EAttackState.SideTilt:
                CheckAttackFrames(m_SideTilt);
                break;
            case EAttackState.UpTilt:
                CheckAttackFrames(m_UpTilt);
                break;
            case EAttackState.DownTilt:
                CheckAttackFrames(m_DownTilt);
                break;
            case EAttackState.ForwardAir:
                CheckAttackFrames(m_ForwardAir);
                break;
            case EAttackState.UpAir:
                CheckAttackFrames(m_UpAir);
                break;
            case EAttackState.DownAir:
                CheckAttackFrames(m_DownAir);
                break;
            case EAttackState.BackAir:
                CheckAttackFrames(m_BackAir);
                break;
            case EAttackState.NeutralAir:
                CheckAttackFrames(m_NeutralAir);
                break;
        }
    }
    #endregion

    #region Functions
    //INTERRUPTION DE L'ATTAQUE
    public void InterruptAttack()
    {
        //On passe dan un �tat o� on n'attaque plus
        m_CurrentAttackState = EAttackState.Nothing;
        //On r�initialise la liste des joueurs touch�s par l'attaque
        m_PlayersHit.Clear();
    }
    public void CheckAttackInput(Vector2 p_JoyStickInput)
    {
        //On calcule l'angle de la direction du joystick par rapport � un vecteur 1,0,0 (vers la droite)
        float l_Angle = Vector2.Angle(new Vector2(1.0f, 0.0f), p_JoyStickInput.normalized);
        l_Angle = l_Angle * Mathf.Sign(m_AimDirection.y);

        if (m_CurrentAttackState == EAttackState.Nothing && m_LastAttack == EAttackState.Nothing)
        {
            if (!m_IsAerial)
            {
                if (p_JoyStickInput.magnitude >= m_JoystickDeadZone)
                {
                    //Vers la gauche
                    if (l_Angle >= 135.0f)
                    {//Penser � retourner le joueur si n�cessaire
                        TurnLeft(true);
                        m_CurrentAttackState = EAttackState.SideTilt;
                        SetMaxAttackDuration(m_SideTilt);
                    }
                    //Vers le haut
                    else if (l_Angle >= 45.0f)
                    {
                        m_CurrentAttackState = EAttackState.UpTilt;
                        SetMaxAttackDuration(m_UpTilt);
                    }
                    //Vers la droite
                    else if (l_Angle >= -45.0f)
                    {//Penser � retourner le joueur si n�cessaire
                        TurnLeft(false);
                        m_CurrentAttackState = EAttackState.SideTilt;
                        SetMaxAttackDuration(m_SideTilt);
                    }
                    //Vers le bas
                    else if (l_Angle >= -135.0f)
                    {
                        m_CurrentAttackState = EAttackState.DownTilt;
                        SetMaxAttackDuration(m_DownTilt);
                    }
                    //Vers la gauche
                    else
                    {//Penser � retourner le joueur si n�cessaire
                        TurnLeft(true);
                        m_CurrentAttackState = EAttackState.SideTilt;
                        SetMaxAttackDuration(m_SideTilt);
                    }
                }
                else
                {
                    m_CurrentAttackState = EAttackState.Jab;
                    SetMaxAttackDuration(m_Jab);
                }

            }
            else
            {
                if (p_JoyStickInput.magnitude >= m_JoystickDeadZone)
                {
                    //Vers la gauche
                    if (l_Angle >= 135.0f)
                    {
                        if (m_PlayerDirection == 1)
                        {
                            m_CurrentAttackState = EAttackState.BackAir;
                            SetMaxAttackDuration(m_BackAir);
                        }
                        else
                        {
                            m_CurrentAttackState = EAttackState.ForwardAir;
                            SetMaxAttackDuration(m_ForwardAir);
                        }
                    }
                    //Vers le haut
                    else if (l_Angle >= 45.0f)
                    {
                        m_CurrentAttackState = EAttackState.UpAir;
                        SetMaxAttackDuration(m_UpAir);
                    }
                    //Vers la droite
                    else if (l_Angle >= -45.0f)
                    {
                        if (m_PlayerDirection == 1)
                        {
                            m_CurrentAttackState = EAttackState.ForwardAir;
                            SetMaxAttackDuration(m_ForwardAir);
                        }
                        else
                        {
                            m_CurrentAttackState = EAttackState.BackAir;
                            SetMaxAttackDuration(m_BackAir);
                        }
                    }
                    //Vers le bas
                    else if (l_Angle >= -135.0f)
                    {
                        m_CurrentAttackState = EAttackState.DownAir;
                        SetMaxAttackDuration(m_DownAir);
                    }
                    //Vers la gauche
                    else
                    {
                        if (m_PlayerDirection == 1)
                        {
                            m_CurrentAttackState = EAttackState.BackAir;
                            SetMaxAttackDuration(m_BackAir);
                        }
                        else
                        {
                            m_CurrentAttackState = EAttackState.ForwardAir;
                            SetMaxAttackDuration(m_ForwardAir);
                        }
                    }
                }
                else
                {
                    m_CurrentAttackState = EAttackState.NeutralAir;
                    SetMaxAttackDuration(m_NeutralAir);
                }


            }
        }

    }
    public void TurnLeft(bool p_Left)
    {
        if (p_Left)
        {
            m_PlayerDirection = -1;
        }
        else
        {
            m_PlayerDirection = 1;
        }
    }
    //CALCUL DE LA DUREE MAX DE L'ATTAQUE
    public void SetMaxAttackDuration(SO_Attack p_AttackStats)
    {
        foreach (SO_Hit l_Hit in p_AttackStats.Hits)
        {
            foreach (SO_HitBox l_HitBox in l_Hit.HitBoxes)
            {
                //On compare quelle hitbox a la dur�e (pre-lag, dur�e, post-lag) la plus �l�v�e
                m_MaxFrameCount = Mathf.Max(m_MaxFrameCount, l_HitBox.BeforeLag + l_HitBox.Duration + p_AttackStats.AfterLag);
            }
        }
        foreach (SO_Projectile l_ProjectileStat in p_AttackStats.Projectiles)
        {
            //On v�rifie aussi si un projectile s'instantie plus tard que le reste
            m_MaxFrameCount = Mathf.Max(m_MaxFrameCount, l_ProjectileStat.InstantiationFrame + p_AttackStats.AfterLag);
        }
    }
    public void CheckAttackFrames(SO_Attack p_AttackStats)
    {
        foreach (SO_Hit l_Hit in p_AttackStats.Hits)
        {
            foreach (SO_HitBox l_HitBox in l_Hit.HitBoxes)
            {
                //On regarde pour chaque Hitbox si elle est cens�e s'activer � la frame o� on est actuellement
                if (m_CurrentFrameCount > l_HitBox.BeforeLag && m_CurrentFrameCount <= (l_HitBox.BeforeLag + l_HitBox.Duration))
                {
                    //Si oui, on l'active
                    CheckEnemiesInRange(l_HitBox, l_Hit);
                }
            }
        }
        //On regarde pour chaque projectile de l'attaque si on est � la frame d'instantiation
        foreach (SO_Projectile l_ProjectileStat in p_AttackStats.Projectiles)
        {
            if (m_CurrentFrameCount == l_ProjectileStat.InstantiationFrame)
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
        l_HitBoxPosition.x = transform.position.x + (p_HitBox.RelativePosition.x * m_PlayerDirection);
        //On r�cup�re tous les objets qui ont un collider qu'on peut attaquer dans la hitbox
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
        {
            //Pour chaque collider trouv� pr�c�demment
            foreach (Collider l_HitObject in l_HitObjects)
            {
                //On v�rifie si c'est un  joueur
                Health l_HitObjectHealth = l_HitObject.GetComponentInChildren<Health>();
                //SI c'est un joueur
                if (l_HitObjectHealth != null)
                {
                    //On regarde si le coup actuellement en cours a d�j� touch� quelqu'un
                    if (m_PlayersHit.TryGetValue(p_Hit, out List<Health> l_HitPlayers))
                    {
                        bool l_AlreadyHit = false;
                        //Si oui, on regarde s'il a d�j� touch� ce joueur
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
                            //Si le coup n'a pas touch� ce joueur on applique les d�g�ts
                            ApplyDamages(p_Hit, p_HitBox, l_HitObjectHealth);
                        }
                    }
                    //Si le coup n'a touch� personne on applique les d�g�ts
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
        //Oninflige les d�g�ts au joueur touch�
        p_PlayerHit.TakeDamages(p_HitBox, m_PlayerDirection);
        //On ajoute le joueur touch� � la liste des joueurs touch�s
        AddPlayerToDictionary(p_Hit, p_PlayerHit);
    }
    public void AddPlayerToDictionary(SO_Hit p_Hit, Health p_HitPlayer)
    {
        //S'il existe d�j� une liste de joueurs touch�s par ce coup, on ajoute le joueur � la liste
        if (m_PlayersHit.TryGetValue(p_Hit, out List<Health> l_HitPlayers))
        {
            l_HitPlayers.Add(p_HitPlayer);
        }
        //Sinon, on cr�e une liste qu'on associe au coup, puis on ajoute le joueur � la liste
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
        l_ProjectilePosition.x = transform.position.x + (p_Projectile.RelativeStartPosition.x * m_PlayerDirection);
        m_InstantiatedProjectile = Instantiate(p_Projectile.ProjectilePrefab, l_ProjectilePosition, Quaternion.identity);
        //On le met dans la bonne direction
        if (m_PlayerDirection == 1)
        {
            //Si le joueur regarde � droite, on garde l'angle de base
            m_InstantiatedProjectile.transform.Rotate(Vector3.forward, p_Projectile.ShootAngle);
        }
        else
        {
            //S'il regarde � gauche, on calcule l'angle correspondant (180� - Angle)
            m_InstantiatedProjectile.transform.Rotate(Vector3.forward, 180.0f - p_Projectile.ShootAngle);
        }
        //On lui donne les cibles qu'il peut attaquer
        m_InstantiatedProjectile.GetComponent<Projectile>().AttackableLayer = m_PlayerInfos.AttackableLayers;
        //Et on lui donne ses statistiques
        m_InstantiatedProjectile.GetComponent<Projectile>().ProjectileStats = p_Projectile;
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        switch (m_CurrentAttackState)
        {
            case EAttackState.Nothing:
                break;
            case EAttackState.Jab:
                GizmosFunction(m_Jab);
                break;
            case EAttackState.SideTilt:
                GizmosFunction(m_SideTilt);
                break;
            case EAttackState.UpTilt:
                GizmosFunction(m_UpTilt);
                break;
            case EAttackState.DownTilt:
                GizmosFunction(m_DownTilt);
                break;
            case EAttackState.ForwardAir:
                GizmosFunction(m_ForwardAir);
                break;
            case EAttackState.UpAir:
                GizmosFunction(m_UpAir);
                break;
            case EAttackState.DownAir:
                GizmosFunction(m_DownAir);
                break;
            case EAttackState.BackAir:
                GizmosFunction(m_BackAir);
                break;
            case EAttackState.NeutralAir:
                GizmosFunction(m_NeutralAir);
                break;
        }
    }
    public void GizmosFunction(SO_Attack p_Attack)
    {
        Gizmos.color = Color.magenta;
        foreach (SO_Hit l_Hit in p_Attack.Hits)
        {
            foreach (SO_HitBox l_HitBox in l_Hit.HitBoxes)
            {
                if (m_CurrentFrameCount > l_HitBox.BeforeLag && m_CurrentFrameCount <= (l_HitBox.BeforeLag + l_HitBox.Duration))
                {
                    Vector3 l_HitBoxPosition = Vector3.zero;
                    l_HitBoxPosition.y = transform.position.y + l_HitBox.RelativePosition.y;
                    l_HitBoxPosition.x = transform.position.x + (l_HitBox.RelativePosition.x * m_PlayerDirection);
                    switch (l_HitBox.HitBoxType)
                    {
                        case EHitBOxType.Square:
                            Gizmos.DrawCube(l_HitBoxPosition, l_HitBox.Size);
                            break;
                        case EHitBOxType.Sphere:
                            Gizmos.DrawSphere(l_HitBoxPosition, l_HitBox.Radius);
                            break;
                    }
                }
            }
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
    UpAir,
}

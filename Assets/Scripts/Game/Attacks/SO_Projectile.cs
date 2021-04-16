using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectileStat", menuName = "ScriptableObjects/Gameplay/NewProjectile")]
public class SO_Projectile : ScriptableObject
{
    [SerializeField]
    private GameObject m_ProjectilePrefab = null;
    public GameObject ProjectilePrefab { get { return m_ProjectilePrefab; } }
    [SerializeField]
    private float m_Damages = 10.0f;
    public float Damages { get { return m_Damages; } }
    [SerializeField]
    private float m_EjectionPower = 10.0f;
    public float EjectionPower { get { return m_EjectionPower; } }
    [SerializeField]
    private float m_EjectionAngle = 10.0f;
    public float EjectionAngle { get { return m_EjectionAngle; } }
    [SerializeField]
    private Vector3 m_RelativeStartPosition = Vector3.zero;
    public Vector3 RelativeStartPosition { get { return m_RelativeStartPosition; } }
    [SerializeField]
    private float m_ShootAngle = 0.0f;
    public float ShootAngle { get { return m_ShootAngle; } }
    [SerializeField]
    private float m_Speed = 10.0f;
    public float Speed { get { return m_Speed; } }
    [SerializeField]
    private int m_InstantiationFrame = 30;
    public int InstantiationFrame { get { return m_InstantiationFrame; } }
}

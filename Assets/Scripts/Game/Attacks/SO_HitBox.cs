using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHitBox", menuName = "ScriptableObjects/Gameplay/Attacks/NewHitBox")]
public class SO_HitBox : ScriptableObject
{
    [Header("SHAPE AND SIZE")]
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
    private float m_HitLag = 0.5f;
    public float HitLag { get { return m_HitLag; } }
    [SerializeField]
    private EHitBOxType m_HitBoxType = EHitBOxType.Square;
    public EHitBOxType HitBoxType { get { return m_HitBoxType; } }
    [Header("Position")]
    [SerializeField]
    private Vector3 m_RelativePosition = Vector3.zero;
    public Vector3 RelativePosition { get { return m_RelativePosition; } }
    [Header("Sphere size")]
    [SerializeField]
    private float m_Radius = 0.5f;
    public float Radius { get { return m_Radius; } }
    [Header("Box size")]
    [SerializeField]
    private Vector3 m_Size = Vector3.one;
    public Vector3 Size { get { return m_Size; } }
    [Header("TIMERS")]
    [SerializeField]
    private int m_BeforeLag = 0;
    public int BeforeLag { get { return m_BeforeLag; } }
    [SerializeField]
    private int m_Duration = 20;
    public int Duration { get { return m_Duration; } }


}

public enum EHitBOxType
{
    Square,
    Sphere,
}

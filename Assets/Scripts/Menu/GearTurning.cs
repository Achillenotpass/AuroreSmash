using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GearTurning : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Gear1 = null;
    [SerializeField]
    private GameObject m_Gear2 = null;

    private void Start()
    {
        Vector3 l_Angle = m_Gear1.transform.rotation.eulerAngles;
        l_Angle.z = l_Angle.z + 180.0f;
        m_Gear1.transform.DORotate(l_Angle, 1.0f).SetLoops(-1, LoopType.Restart);

        l_Angle = m_Gear2.transform.rotation.eulerAngles;
        l_Angle.z = l_Angle.z - 180.0f;
        m_Gear2.transform.DORotate(l_Angle, 1.0f).SetLoops(-1, LoopType.Restart);
    }
}

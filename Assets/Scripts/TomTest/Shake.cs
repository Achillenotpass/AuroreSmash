using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour, IUpdateUser
{
    #region CustomUpdate
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

    #region Update
    public void CustomUpdate(float p_DeltaTime)
    {

    }
    #endregion

    public IEnumerator CreateShake(float p_ShakePower, float p_ShakeDuration)
    {
        Vector3 l_OriginalPos = transform.localPosition;

        float l_elapsed = 0.0f;

        while(l_elapsed < p_ShakeDuration)
        {
            float x = Random.Range(-1f, 1f) * p_ShakePower;
            float y = Random.Range(-1f, 1f) * p_ShakePower;

            transform.localPosition = new Vector3(x, y, l_OriginalPos.z);

            l_elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = l_OriginalPos;
    }
}

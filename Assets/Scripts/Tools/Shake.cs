using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public IEnumerator CreateShake(float p_ShakePower, float p_ShakeDuration)
    {

        float l_elapsed = 0.0f;

        while(l_elapsed < p_ShakeDuration)
        {
            float l_X = Random.Range(-1f, 1f) * p_ShakePower;
            float l_Y = Random.Range(-1f, 1f) * p_ShakePower;

            transform.localPosition += new Vector3(l_X, l_Y, 0);

            l_elapsed += Time.deltaTime;

            yield return null;
        }
    }
}
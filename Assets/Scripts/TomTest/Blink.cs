using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    public IEnumerator Blinking(GameObject p_ObjectToBlink, float p_BlinkDuration, float p_BlinkSpeed, float p_BlinkIntesity)
    {
        float l_BlinkTimer = p_BlinkDuration;
        Debug.Log("aaa");
        while (l_BlinkTimer > 0)
        {
            p_ObjectToBlink.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, p_BlinkIntesity);
            yield return new WaitForSeconds(p_BlinkSpeed);
            l_BlinkTimer = l_BlinkTimer - p_BlinkSpeed;
            p_ObjectToBlink.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1);
            if (l_BlinkTimer <= 0)
                break;
            yield return new WaitForSeconds(p_BlinkSpeed);
            l_BlinkTimer = l_BlinkTimer - p_BlinkSpeed;
        }
    }
}

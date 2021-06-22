using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Don't forget to make the script derive from IUpdateUser
public class Translator : MonoBehaviour, IUpdateUser
{
    private PlayerInfos m_PlayerInfos = null;
    private Vector2 m_Direction = Vector2.zero;

    //Everything from here,
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
    //to here is necessary for the Update manager
    

    
    //Use this function instead of the usual update
    public void CustomUpdate(float p_DeltaTime)
    {
        transform.Translate(new Vector3(m_Direction.x, m_Direction.y, 0.0f) * p_DeltaTime);
    }


    public void mescouilles(InputAction.CallbackContext p_Context)
    {
        m_Direction = p_Context.ReadValue<Vector2>();
    }
}

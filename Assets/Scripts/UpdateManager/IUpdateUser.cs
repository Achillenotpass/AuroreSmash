public interface IUpdateUser
{
    //This need to be put in every script that uses the update manager
    /*[SerializeField]
    private SO_UpdateLayerSettings m_UpdateSettings = null;
    private void OnEnable()
    {
        m_UpdateSettings.Bind(this);
    }
    private void OnDisable()
    {
        m_UpdateSettings.Unbind(this);
    }*/
    void CustomUpdate(float p_DeltaTime);
}

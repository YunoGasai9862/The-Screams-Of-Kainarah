using System;
using System.Threading.Tasks;
using UnityEngine.Events;

public class PlayerPowerUpModeEvent: UnityEventWTAsync<float>
{
    private UnityEvent<float> m_playerPowerUpModeEvent = new UnityEvent<float>();
    public override UnityEvent<float> GetInstance()
    {
        return m_playerPowerUpModeEvent;
    }
    public override Task AddListener(UnityAction<float> unityAction)
    {
        try
        {
            GetInstance().AddListener(unityAction);
        }
        catch(Exception ex)
        {
            return Task.FromException(ex);
        }

        return Task.CompletedTask;
    }

    public override Task Invoke(float value)
    {
        m_playerPowerUpModeEvent.Invoke(value);

        return Task.CompletedTask;
    }
}
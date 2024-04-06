using System;
using System.Threading.Tasks;
using UnityEngine.Events;

public class PlayerPowerUpModeEvent: UnityEventWT<float>
{
    private UnityEvent<float> _playerPowerUpModeEvent = new UnityEvent<float>();
    public override UnityEvent<float> GetInstance()
    {
        return _playerPowerUpModeEvent;
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
}
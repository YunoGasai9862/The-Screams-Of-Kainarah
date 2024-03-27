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
        GetInstance().AddListener(unityAction);
        

        //return anything here (an indication)
        return Task.CompletedTask;
    }
}
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class PlayerBoostAttackEvent: UnityEventWTAsync<bool>
{

    private UnityEvent<bool> PlayerBoostAttack { get; set; } = new UnityEvent<bool>();
    public override Task AddListener(UnityAction<bool> action)
    {
        PlayerBoostAttack.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<bool> GetInstance()
    {
        return PlayerBoostAttack;
    }

    public override Task Invoke(bool value)
    {
        PlayerBoostAttack.Invoke(value);

        return Task.CompletedTask;
    }
}

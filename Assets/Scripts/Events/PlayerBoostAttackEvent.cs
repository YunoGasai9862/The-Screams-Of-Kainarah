using UnityEngine;
using UnityEngine.Events;

public class PlayerBoostAttackEvent: UnityEventWT<bool, Animator>
{
    public UnityEvent<bool, Animator> _playerBoostAttackEvent = new UnityEvent<bool, Animator>();
    public bool ShouldBoostAttack { get; set; }
    public PlayerBoostAttackEvent() { }

    public override UnityEvent<bool, Animator> GetInstance()
    {
        return _playerBoostAttackEvent;
    }
    public override void AddListener(UnityAction<bool, Animator> action)
    {
        _playerBoostAttackEvent.AddListener(action);
    }
}

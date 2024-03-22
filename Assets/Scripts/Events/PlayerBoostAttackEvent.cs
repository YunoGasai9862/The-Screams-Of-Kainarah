using UnityEngine.Events;

public class PlayerBoostAttackEvent: UnityEventWT<bool>
{
    PlayerBoostAttackEvent() { }

    public override UnityEvent<bool> GetInstance()
    {
        throw new System.NotImplementedException();
    }
}

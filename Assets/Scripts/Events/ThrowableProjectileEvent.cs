using UnityEngine.Events;

public class ThrowableProjectileEvent: UnityEvent<bool>
{
    private bool _canThrow;
    public bool CanThrow { get => _canThrow; set => _canThrow = value; }
    public ThrowableProjectileEvent(bool canThrow)
    {
        _canThrow = canThrow;
    }
    public ThrowableProjectileEvent()
    {

    }

}
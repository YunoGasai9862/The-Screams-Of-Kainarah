using UnityEngine.Events;

public class DaggerOnThrowEvent: UnityEvent<bool>
{
    private bool _isDaggerInMotion;
    public bool DaggerInMotion {get => _isDaggerInMotion; set => _isDaggerInMotion = value; }
    public DaggerOnThrowEvent(bool isDaggerInMotion)
    {
        DaggerInMotion = isDaggerInMotion;
    }
    public DaggerOnThrowEvent() { }


}
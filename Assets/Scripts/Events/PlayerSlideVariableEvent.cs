using UnityEngine.Events;

public class PlayerSlideVariableEvent : UnityEvent<bool>
{
    private PlayerSlideStateEvent _playerSlideEventState;
    public bool PlayerFinishedSliding { get; private set; } = false;
    public PlayerSlideVariableEvent() { _playerSlideEventState = new PlayerSlideStateEvent(); _playerSlideEventState.AddListener(SetPlayerSlideState); }
    public void SetPlayerSlideState(bool slideState)
    {
        PlayerFinishedSliding = slideState;
    }

    public void PlayerSlideStateEventInvoke(bool slideState)
    {
        _playerSlideEventState.Invoke(slideState);
    }
}

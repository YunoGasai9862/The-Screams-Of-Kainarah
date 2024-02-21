using UnityEngine.Events;

public class PlayerJumpHeightEvent : UnityEvent<float>
{
    public float MaxTime {set; get;}
    public PlayerJumpHeightEvent(float maxTime)
    {
        MaxTime = maxTime;
    }

    public void ShouldFall(float timeEclipsed)
    {
        if (timeEclipsed > MaxTime)
        {
            //pass a variable from jumpingController, to force the player to fall. Put it in the conditional too
        }
    }
}

using UnityEngine.Events;
using System.Threading.Tasks;
using System.Diagnostics;
public class PlayerJumpTimeEvent : UnityEvent<bool>
{
    public float MaxTime {set; get;}

    public bool Fall { set; get; }
    public PlayerJumpTimeEvent(float maxTime)
    {
        MaxTime = maxTime;
    }

    public void ShouldFall(float timeEclipsed)
    {
        if (timeEclipsed > MaxTime)
        {
            Invoke(true);
        }
    }
}

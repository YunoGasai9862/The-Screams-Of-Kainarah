
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PowerMode : MonoBehaviour
{
    [SerializeField] public Image fill;
    [SerializeField] public Slider slide;
    [SerializeField] public Gradient gradient;
    [SerializeField] public PlayerPowerUpModeEvent powerUpEvent;

    private const float MAX_POWER_UP_POINTS = 100f;
    public float PowerUpPoints { get; set; } = 0;

    // Start is called before the first frame update
    void Start()
    {
        powerUpEvent.AddListener(UpdatePowerUpPoints);   
    }

    private async void UpdatePowerUpPoints(float powerUpPoints)
    {
        if (PowerUpPoints + powerUpPoints <= MAX_POWER_UP_POINTS)
            PowerUpPoints += powerUpPoints;

        await UpdateSliderValue(PowerUpPoints);

    }
    private Task SetGradientValue(Gradient gradient)
    {
        //use gradient
        return Task.CompletedTask;
    }

    private Task UpdateSliderValue(float sliderValue)
    {
        //use slider
        slide.value = sliderValue;
        return Task.CompletedTask;
    }


}

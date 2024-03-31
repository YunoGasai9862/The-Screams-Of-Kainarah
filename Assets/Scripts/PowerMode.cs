
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PowerMode : MonoBehaviour
{
    [SerializeField] public Image fill;
    [SerializeField] public Slider slider;
    [SerializeField] public Gradient gradient;
    [SerializeField] public PlayerPowerUpModeEvent powerUpEvent;
    [SerializeField] public PowerUpBarFillEvent powerUpBarFillEvent;

    private const float MAX_POWER_UP_POINTS = 100f;
    public float PowerUpPoints { get; set; } = 0;
    public float PointsDifference { get; set; } = 0f;

    void Start()
    {
        powerUpEvent.AddListener(NotifyPowerUpBar);
        fill.color = gradient.Evaluate(slider.value / 100.0f);
    }

    private async void NotifyPowerUpBar(float powerUpPoints)
    {
        await UpdatePowerUpPoints(powerUpPoints);

        await UpdateSliderValue(PowerUpPoints);

        await SetGradientValue(gradient, slider);
    }
    private Task SetGradientValue(Gradient gradient, Slider slider)
    {
        Debug.Log(slider.value);
        fill.color = gradient.Evaluate(slider.value / 100.0f);
        return Task.CompletedTask;
    }

    private Task UpdateSliderValue(float sliderValue)
    {
        slider.value = sliderValue;
        return Task.CompletedTask;
    }
    private Task UpdatePowerUpPoints(float powerUpPoints)
    {
        if (PowerUpPoints + powerUpPoints >= MAX_POWER_UP_POINTS)
        {
            PointsDifference = MAX_POWER_UP_POINTS - PowerUpPoints;
            powerUpBarFillEvent.GetInstance().Invoke(true);
        }
        else
        {
            PointsDifference = powerUpPoints;
            powerUpBarFillEvent.GetInstance().Invoke(false);
        }

        PowerUpPoints += PointsDifference;
        return Task.CompletedTask;
    }


}

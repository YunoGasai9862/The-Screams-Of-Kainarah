using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class PowerMode : MonoBehaviour
{
    private const float MAX_POWER_UP_POINTS = 100f;

    [SerializeField] Image fill;
    [SerializeField] Slider slider;
    [SerializeField] Gradient gradient;
    [SerializeField] PlayerPowerUpModeEvent powerUpEvent;

    public float PowerUpPoints { get; set; } = 0;

    // Start is called before the first frame update
    void Start()
    {
        powerUpEvent.AddListener(UpdatePowerUpPoints);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdatePowerUpPoints(float powerUpPoints)
    {
        if (PowerUpPoints + powerUpPoints <= MAX_POWER_UP_POINTS)
            PowerUpPoints += powerUpPoints;

    }
    private Task SetGradientValue(Gradient gradient)
    {
        //use gradient
        return Task.CompletedTask;
    }

    private Task UpdateSliderValue(float sliderValue)
    {
        //use slider

        return Task.CompletedTask;
    }


}

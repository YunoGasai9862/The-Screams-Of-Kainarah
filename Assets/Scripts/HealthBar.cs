using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    [SerializeField] Image Fill;
    [SerializeField] Slider slide;
    [SerializeField] Gradient gr;
    [SerializeField] string TargetHealth;
    private void Start()
    {
      
        Fill.color = gr.Evaluate(slide.value);
        
    }
    void Update()
    {
        TrackHealth(TargetHealth);

    }


    private void TrackHealth(string TH)
    {
     
         switch(TH)
        {
            case "Player":
                slide.value = HealthTracker.ReturnMainPlayerHealth();
                break;

            case "Boss":
                slide.value = HealthTracker.ReturnBossHealth();
                break;

        }

        Fill.color = gr.Evaluate(slide.value / 100.0f);

    }



}

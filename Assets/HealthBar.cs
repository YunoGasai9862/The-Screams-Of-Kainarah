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
    private ArrayList arr;
    private void Start()
    {
      
        Fill.color = gr.Evaluate(slide.value);
        arr=new ArrayList();
    }
    void Update()
    {
        if(TargetHealth == "Player")
        {
          
            TrackHealth(TargetHealth);
            arr.Add(TargetHealth);
          
        }

        if(TargetHealth == "Boss")
        {
            TrackHealth(TargetHealth);
            arr.Add(TargetHealth);

        }

    }


    private void TrackHealth(string TH)
    {
     
          switch(TH)
        {
            case "Player":
                slide.value = HealthTracker.ReturnBossHealth();
                Fill.color = gr.Evaluate(slide.value / 100.0f);
                break;
        }
        
           
        
       
    }

   

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    [SerializeField] Image Fill;
    [SerializeField] Slider slide;
    [SerializeField] Gradient gr;

    private void Start()
    {

        Fill.color = gr.Evaluate(slide.value);
    }
    void Update()
    {
        slide.value = (float)Movement.MAXHEALTH;
        Fill.color = gr.Evaluate(slide.value / 100.0f);
    }
}

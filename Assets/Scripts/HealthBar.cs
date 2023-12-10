using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    [SerializeField] Image Fill;
    [SerializeField] Slider slide;
    [SerializeField] Gradient gr;
    [SerializeField] string TargetEntityTag;

    private AbstractEntity _targetEntity;
    private void Start()
    {
        _targetEntity = GameObject.FindGameObjectWithTag(TargetEntityTag).GetComponent<AbstractEntity>();
        Fill.color = gr.Evaluate(slide.value);
    }
    void Update()
    {
        TrackHealth(_targetEntity);      
    }

    private void TrackHealth(AbstractEntity abstractEntity)
    {
        slide.value = abstractEntity.Health;
        Debug.Log((abstractEntity, slide.value));
        Fill.color = gr.Evaluate(slide.value / 100.0f);
    }

}

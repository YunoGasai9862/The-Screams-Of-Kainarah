using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    [SerializeField] Image Fill;
    [SerializeField] Slider slide;
    [SerializeField] Gradient gr;
    [SerializeField] string TargetEntityTag;

    private AbstractEntity _targetEntity;
    private GameObject _targetGameObject;
    private void Start()
    {
        _targetGameObject = GameObject.FindGameObjectWithTag(TargetEntityTag);
        if (_targetGameObject != null)
            _targetEntity=_targetGameObject.GetComponent<AbstractEntity>();

        Fill.color = gr.Evaluate(slide.value);
    }
    void Update()
    {
        if(_targetGameObject==null)
        {
            _targetGameObject = GameObject.FindGameObjectWithTag(TargetEntityTag);
            _targetEntity = _targetGameObject.GetComponent<AbstractEntity>();
        }
        
        if(_targetEntity!=null)
           TrackHealth(_targetEntity);      
    }

    private void TrackHealth(AbstractEntity abstractEntity)
    {
        slide.value = abstractEntity.Health;
        Fill.color = gr.Evaluate(slide.value / 100.0f);
    }

}

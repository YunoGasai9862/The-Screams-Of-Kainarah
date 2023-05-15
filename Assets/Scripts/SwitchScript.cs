
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class SwitchScript : MonoBehaviour
{
    private Toggle _toggle;
    [SerializeField] RectTransform _uiHandler;
    [SerializeField] Color _color;
    private Vector2 AnchorPoint;
    void Start()
    {
        _toggle= GetComponent<Toggle>();   
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_toggle.isOn)
        {
            AnchorPoint = _uiHandler.anchoredPosition;
            _uiHandler.DOAnchorPos(AnchorPoint * -1, 1f);
        }
    }
}

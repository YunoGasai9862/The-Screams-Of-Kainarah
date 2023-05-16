
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting;

public class SwitchScript : MonoBehaviour
{
    private Toggle _toggle;
    [SerializeField] RectTransform _uiHandler;
    [SerializeField] Image BG;
    [SerializeField] Image HandlerColor;
    [SerializeField] Color HandleColor;
    [SerializeField] Color _background;
    [SerializeField] Color _transitionBackground;
    [SerializeField] Color _transitionHandler;

    private Vector2 AnchorPoint;
 
    private void Awake()
    {
        _toggle = GetComponent<Toggle>();

        AnchorPoint = _uiHandler.anchoredPosition;
        BG.color = _background;
        HandlerColor.color = HandleColor;
        _toggle.onValueChanged.AddListener(ChangeToggle); //delegates/adds listener

        if(_toggle.isOn)
        {
            ChangeToggle(true);  //if its true, passes true
            //when false, it automatically delegates the false one
        }
    }
    // Update is called once per frame
  
    public void ChangeToggle(bool isOn)
    {
        _uiHandler.DOAnchorPos(isOn ? AnchorPoint * -1 : AnchorPoint,.4f).SetEase(Ease.InFlash);
        BG.DOColor(isOn ? _transitionBackground : _background, .4f).SetEase(Ease.InFlash);
        HandlerColor.DOColor(isOn ? _transitionHandler : HandleColor, .4f).SetEase(Ease.InFlash);

    }
}

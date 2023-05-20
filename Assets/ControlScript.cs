using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator _anim;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _anim.SetBool("Zoom", true);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _anim.SetBool("Zoom", false);

    }





}

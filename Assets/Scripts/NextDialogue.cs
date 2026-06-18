using Assets.Scripts.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NextDialogue : MonoBehaviorScene, IPointerClickHandler
{
    [SerializeField]
    NextDialogueTriggerEvent nextDialogueTriggerEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        nextDialogueTriggerEvent.Invoke(true);
    }
}

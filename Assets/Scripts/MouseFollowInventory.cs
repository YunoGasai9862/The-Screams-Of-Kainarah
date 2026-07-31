using Assets.Scripts.BaseScene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollowInventory : MonoBehaviorScene
{
    [SerializeField] GameObject starAura;
    [SerializeField] Camera otherCamera;

    void Update()
    {
        Vector3 MousePosition= otherCamera.ScreenToWorldPoint(Input.mousePosition); //words perfectly
        starAura.transform.position = MousePosition;
    }
}

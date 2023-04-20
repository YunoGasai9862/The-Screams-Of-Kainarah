using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollowInventory : MonoBehaviour
{
    [SerializeField] GameObject starAura;
    [SerializeField] Camera otherCamera;



    // Update is called once per frame
    void Update()
    {
        Vector2 MousePosition= otherCamera.ScreenToWorldPoint(Input.mousePosition); //words perfectly
        starAura.transform.position = MousePosition;
    }
}

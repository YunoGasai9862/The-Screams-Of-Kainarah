using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EvenTriggers : MonoBehaviour
{
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    public void ZoomUp()
    {
        _animator.SetBool("Zoom", true);
    }

    public void ZoomDown()
    {
        _animator.SetBool("Zoom", false);

    }

    public void OpenControlsPanel()
    {

    }
}

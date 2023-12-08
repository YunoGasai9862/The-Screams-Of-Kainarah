using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera _waterCamera;
    [Header("Aspect Ratio:")]
    public float aspectRatio;

    private void Awake()
    {
        _waterCamera = GetComponent<Camera>();
    }
    void Start()
    {
        //fix this tomorrow
        _waterCamera.aspect = aspectRatio;
    }

   
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera _waterCamera;
    [Header("Aspect Ratio:")]
    public float aspectRatio;
    public GameObject waterSprite;


    private void Awake()
    {
        _waterCamera = GetComponent<Camera>();
    }
    void Start()
    {
        _waterCamera.aspect *= aspectRatio;
        waterSprite.transform.localScale = new Vector3(waterSprite.transform.localScale.x * aspectRatio, waterSprite.transform.localScale.y, waterSprite.transform.localScale.z); //so it stretches the same size
    }

   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingBosses : MonoBehaviour
{
    private float camHeight, CamWidth;
    [SerializeField] float CameraMinSize;
    [SerializeField] GameObject Boss;
    [SerializeField] GameObject Health;
    private bool openHealthBar = false;
    void Start()
    {
        camHeight = 2 * (Camera.main.orthographicSize);  //gives you half the height
        CamWidth = camHeight * Camera.main.aspect;  //gives you half of its width by multiplying camera's aspect ratio
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, Boss.transform.position) <= CamWidth / 2)
        {
            openHealthBar = true;
        }
        if(openHealthBar)
        {
            Health.gameObject.SetActive(true);
        }
    }
}

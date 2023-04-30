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
    public static bool BossExists = false;
    void Start()
    {
        camHeight = 2 * (Camera.main.orthographicSize);  //gives you half the height
        CamWidth = camHeight * Camera.main.aspect;  //gives you half of its width by multiplying camera's aspect ratio
    }

    // Update is called once per frame
    void Update()
    {
        if(Boss!=null)
        {
            if (Vector2.Distance(transform.position, Boss.transform.position) <= CamWidth / 2)
            {
                openHealthBar = true;
                BossExists = true;
            }else
            {
                openHealthBar = false;
                 BossExists = false;

                Health.gameObject.SetActive(false);
            }
            if (openHealthBar)
            {
                Health.gameObject.SetActive(true);
            }
        }
        else
        {
            Health.gameObject.SetActive(false);
            BossExists = false;

        }


    }
}

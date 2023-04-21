using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrackInventoryObjects : MonoBehaviour
{


    [SerializeField] GameObject Panel;
    private CreateInventorySystem _CIS;

   

    // Start is called before the first frame update
    void Start()
    {
        _CIS=transform.GetComponent<CreateInventorySystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}

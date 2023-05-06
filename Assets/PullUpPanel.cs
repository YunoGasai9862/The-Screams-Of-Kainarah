using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullUpPanel : MonoBehaviour
{
    private Animator _anim;

    void Start()
    {
        _anim= GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(TriggerHandler.Failure)
        {
            _anim.SetBool("SufficientFunds", false);
        }
    }

    IEnumerator waiterFunc()
    {
        yield return null;  
    }
}

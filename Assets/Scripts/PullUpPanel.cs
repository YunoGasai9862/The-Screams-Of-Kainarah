using System.Collections;
using UnityEngine;

public class PullUpPanel : MonoBehaviour
{
    private Animator _anim;
    private bool _closePanel = false;

    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!TriggerHandler.Failure && !_closePanel)
        {
            _anim.SetBool("SufficientFunds", false);
            _closePanel = true;
            StartCoroutine(Reset());
        }
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(1f);
        _closePanel = false;
        _anim.SetBool("SufficientFunds", true);
        TriggerHandler.Failure = true;
    }
}

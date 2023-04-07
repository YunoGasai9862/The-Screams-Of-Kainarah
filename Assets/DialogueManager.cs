using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> _storylineSentences;
    void Start()
    {
        _storylineSentences= new Queue<string>();
    }



    public void StartDialogue(DialogueManager dialogue)
    {
        Debug.Log("Starting conversation with" + dialogue.name);
    }

    
}

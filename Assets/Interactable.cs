using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Dialogues dialogue;

    private void Start()
    {
        Invoke("TriggerDialogue", .1f); //because queue is already empty, thats why using Invoke to give some time to the queue
    }

    public void TriggerDialogue()
    {

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);

        
    }
}

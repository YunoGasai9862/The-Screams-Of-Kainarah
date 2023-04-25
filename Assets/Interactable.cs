using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Dialogues dialogue;
    public Dialogues BossDialogue;


    [Header("Conversation between Vendor and the Player")]
    public Dialogues[] WizardPlayerConvo;



    private void Start()
    {
        StartCoroutine(TriggerDialogue(dialogue));//because queue is already empty, thats why using Invoke to give some time to the queue
    }

   public IEnumerator TriggerDialogue(Dialogues dialogue)
    {
        yield return new WaitForSeconds(.1f);
       
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue);


    }

    public IEnumerator TriggerDialogue(Dialogues[] dialogue)
    {
      
            foreach (Dialogues WPC in dialogue) //a converstaion
            {
                FindObjectOfType<DialogueManager>().StartDialogue(WPC);
            }

        yield return null;

    }
 



}

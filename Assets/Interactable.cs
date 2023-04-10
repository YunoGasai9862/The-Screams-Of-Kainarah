using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Dialogues dialogue;
    public Dialogues BossDialogue;

    private bool FoundTheObject = false;

    private void Start()
    {
        StartCoroutine(TriggerDialogue(dialogue));//because queue is already empty, thats why using Invoke to give some time to the queue
    }

   public IEnumerator TriggerDialogue(Dialogues dialogue)
    {
        yield return new WaitForSeconds(.1f);
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);

    }


   public IEnumerator TriggerDialogue(Dialogues dialogue, GameObject source, GameObject target, string tag)
    {

        if(!FoundTheObject)
        {
            RaycastHit2D ray;
            ray = Physics2D.Raycast(source.transform.position, target.transform.position, 5f);
            if (ray.collider != null && ray.collider.CompareTag(tag))
            {
                FoundTheObject = true;
                FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
            }

           
        }
       
        yield return null;

    }


}

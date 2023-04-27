using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DialogueManager : MonoBehaviour
{
    private Queue<string> _storylineSentences;
    public TextMeshProUGUI myname;
    public TextMeshProUGUI maindialogue;
    public Animator myanimator;
    public static bool IsOpen = false;

    private static Dialogues[] _dialogues = null;

    void Start()
    {
       
        _storylineSentences= new Queue<string>();
    }

    public void StartDialogue(Dialogues dialogue,Dialogues[] dialogues=null)
    {
    
             if(dialogues!=null)
             {
                 _dialogues = dialogues;
               
             }
        IsOpen = true;
        myanimator.SetBool("IsOpen", true);
        _storylineSentences.Clear();  //clears the previous dialogues, if there are any

        myname.text = dialogue.playername;
        foreach(string sentence in dialogue.sentences)
        {
            _storylineSentences.Enqueue(sentence);
        }

        DisplayNextSentence();
       
    }

    IEnumerator AnimateLetters(string sentence)
    {
        maindialogue.text = string.Empty;

        for (int i = 0; i < sentence.Length; i++)
        {
            yield return new WaitForSeconds(.04f);
            maindialogue.text += sentence[i];
        }
    }
    public void DisplayNextSentence()
    {
        if (_storylineSentences.Count==0) //if there's nothing in the queue
        {
            if (_dialogues != null && Interactable.MultipleDialogues[_dialogues] == false)
            {
                StartCoroutine(Interactable.TriggerDialogue(_dialogues));
            }
            else
            {
                EndDialogue();
                return;  //exits function
            }
              
        }
        string sentence = _storylineSentences.Dequeue();
        //if the user clicks on the continue earlier, it will stop all the coroutines and start with the new one=>new text
        StopAllCoroutines();

        StartCoroutine(AnimateLetters(sentence));
    }

    void EndDialogue()
    {

        myanimator.SetBool("IsOpen", false);
        IsOpen = false;
    }


}

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
    void Start()
    {
        _storylineSentences= new Queue<string>();
    }



    public void StartDialogue(Dialogues dialogue)
    {
        IsOpen = true;
        myanimator.SetBool("IsOpen", true);
        _storylineSentences.Clear();  //clears the previous dialogues, if there are any

        myname.text = dialogue.playername;

        foreach(string sentence in dialogue.sentences) //adds all the sentences into the queue
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

        if(_storylineSentences.Count==0) //if there's nothing in the queue
        {
            EndDialogue();
            return;  //exits function
        }

        string sentence = _storylineSentences.Dequeue();

        StopAllCoroutines(); //if the user clicks on the continue earlier, it will stop all the coroutines and start with the new one=>new text

        StartCoroutine(AnimateLetters(sentence));
    }

    void EndDialogue()
    {

        myanimator.SetBool("IsOpen", false);
        IsOpen = false;

    }


}

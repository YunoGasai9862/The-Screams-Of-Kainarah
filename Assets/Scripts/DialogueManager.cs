using Amazon.Polly;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DialogueManager : MonoBehaviour
{
    private Queue<string> m_storylineSentences;
    public TextMeshProUGUI myname;
    public Text maindialogue;
    public Animator myanimator;
    public bool isOpen = false;

    private static Dialogues[] m_dialogues = null;

    [SerializeField]
    public AWSPollyDialogueTriggerEvent m_AWSPollyDialogueTriggerEvent;

    //use event for IsOpen as well!
    void Start()
    {
        m_storylineSentences = new Queue<string>();
    }

    public void StartDialogue(Dialogues dialogue, Dialogues[] dialogues = null)
    {

        if (dialogues != null)
        {
            m_dialogues = dialogues;

        }
        isOpen = true;
        myanimator.SetBool("IsOpen", true);
        m_storylineSentences.Clear();  //clears the previous dialogues, if there are any

        myname.text = dialogue.playername;
        foreach (string sentence in dialogue.sentences)
        {
            m_storylineSentences.Enqueue(sentence);
        }

        DisplayNextSentence();

    }
    public bool IsOpen()
    {
        return isOpen;
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
        if (m_storylineSentences.Count == 0) //if there's nothing in the queue
        {

            if (m_dialogues != null && Conversations.MultipleDialogues[m_dialogues] == false)
            {

                StartCoroutine(Conversations.TriggerDialogue(m_dialogues));
                return;  //THIS WAS ALL I NEEDED, OMG!
                //it fixed the issue, oh lord. 
                //the problem was: it was trying to execute the rest of the code without exiting the function
            }
            else
            {
                EndDialogue();
                return;  //exits function
            }

        }

        string dialogue = m_storylineSentences.Dequeue();

        //create a new class when refactoring to send in voices accordingly
        m_AWSPollyDialogueTriggerEvent.Invoke(dialogue, VoiceId.Bianca);
        //if the user clicks on the continue earlier, it will stop all the coroutines and start with the new one=>new text
        StopAllCoroutines();

        StartCoroutine(AnimateLetters(dialogue));


    }
    void EndDialogue()
    {
        myanimator.SetBool("IsOpen", false);
        isOpen = false;
    }


}

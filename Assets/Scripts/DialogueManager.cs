using Amazon.Polly;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DialogueManager : MonoBehaviour
{

    private const string DIALOGUE_ANIMATION_NAME = "IsOpen";

    private Queue<string> m_storylineSentences;
    public TextMeshProUGUI myname;
    public Text maindialogue;
    public Animator myanimator;

    private static Dialogues[] m_dialogues = null;

    [SerializeField]
    public AWSPollyDialogueTriggerEvent AWSPollyDialogueTriggerEvent;
    public DialogueTakingPlaceEvent dialogueTakingPlaceEvent;

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

        myanimator.SetBool(DIALOGUE_ANIMATION_NAME, true);
        dialogueTakingPlaceEvent.Invoke(true);

        m_storylineSentences.Clear();  //clears the previous dialogues, if there are any
        myname.text = dialogue.entityName;

        foreach (string sentence in dialogue.sentences)
        {
            m_storylineSentences.Enqueue(sentence);
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
        if (m_storylineSentences.Count == 0) //if there's nothing in the queue
        {

            if (m_dialogues != null && !Conversations.MultipleDialogues[m_dialogues].dialogueConcluded)
            {

                StartCoroutine(DialogueTriggerManager.TriggerDialogue(m_dialogues));
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
        AWSPollyDialogueTriggerEvent.Invoke(dialogue, VoiceId.Emma);
        //if the user clicks on the continue earlier, it will stop all the coroutines and start with the new one=>new text
        StopAllCoroutines();

        StartCoroutine(AnimateLetters(dialogue));
    }

    void EndDialogue()
    {
        myanimator.SetBool(DIALOGUE_ANIMATION_NAME, false);
        dialogueTakingPlaceEvent.Invoke(true);
    }


}

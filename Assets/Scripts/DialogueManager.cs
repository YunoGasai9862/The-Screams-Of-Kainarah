using Amazon.Polly;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

    private bool NextDialogue { get; set; } = false;

    [SerializeField]
    public AWSPollyDialogueTriggerEvent AWSPollyDialogueTriggerEvent;
    public DialogueTakingPlaceEvent dialogueTakingPlaceEvent;
    public NextDialogueTriggerEvent nextDialogueTriggerEvent;
    //use this new logic now

    void Start()
    {
        m_storylineSentences = new Queue<string>();
    }

    public void PrepareDialogueQueue(Dialogues dialogue)
    {
        myanimator.SetBool(DIALOGUE_ANIMATION_NAME, true);

        dialogueTakingPlaceEvent.Invoke(true);

        m_storylineSentences.Clear();  //clears the previous dialogues, if there are any

        myname.text = dialogue.EntityName;

        foreach (string sentence in dialogue.Sentences)
        {
            m_storylineSentences.Enqueue(sentence);
        }

    }

    private IEnumerator AnimateLetters(string sentence)
    {
        maindialogue.text = string.Empty;

        for (int i = 0; i < sentence.Length; i++)
        {
            yield return new WaitForSeconds(.05f);
            maindialogue.text += sentence[i];
        }

    }

    public Task InvokeAIVoiceEvent(AWSPollyDialogueTriggerEvent awsPollyDialogueTriggerEvent, string sentence, VoiceId voiceId)
    {
        //once all the letters have animated, invoke voice
        awsPollyDialogueTriggerEvent.Invoke(sentence, voiceId);

        return Task.CompletedTask;
    }
    //do it recursively here instead
    public IEnumerator StartDialogue(SemaphoreSlim dialogueSemaphore)
    {

        if (m_storylineSentences.Count == 0) //if there's nothing in the queue
        {
            EndDialogue();

            dialogueSemaphore.Release();

            yield return null;  
        }

        string dialogue = m_storylineSentences.Dequeue();

        InvokeAIVoiceEvent(AWSPollyDialogueTriggerEvent, dialogue, VoiceId.Emma);

        StopAllCoroutines();

        StartCoroutine(AnimateLetters(dialogue));

        //call it again, recursively only if the button is pressed

        yield return new WaitUntil(() => NextDialogue == true);

        StartCoroutine(StartDialogue(dialogueSemaphore));

    }

    void EndDialogue()
    {
        myanimator.SetBool(DIALOGUE_ANIMATION_NAME, false);
        dialogueTakingPlaceEvent.Invoke(false);
    }


}

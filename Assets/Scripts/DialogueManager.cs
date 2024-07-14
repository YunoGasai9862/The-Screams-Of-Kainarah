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

    void Start()
    {
        m_storylineSentences = new Queue<string>();

        nextDialogueTriggerEvent.AddListener(ShouldProceedToNextDialogue);
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
        //fix this - it shouldn't keep animation previous sentences
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

        if (m_storylineSentences.Count == 0) 
        {
            EndDialogue();

            dialogueSemaphore.Release();

            yield return null;  
        }

        NextDialogue = false;

        string dialogue = m_storylineSentences.Dequeue();

        InvokeAIVoiceEvent(AWSPollyDialogueTriggerEvent, dialogue, VoiceId.Emma);

        // StopAllCoroutines(); //find a way to fix that - if the animation is in progress, skip altogether and
        //jump to the next one better approach

        StartCoroutine(AnimateLetters(dialogue));

        yield return new WaitUntil(() => NextDialogue == true);

        StartCoroutine(StartDialogue(dialogueSemaphore));

    }

    private void EndDialogue()
    {
        myanimator.SetBool(DIALOGUE_ANIMATION_NAME, false);
        dialogueTakingPlaceEvent.Invoke(false);
    }

    private void ShouldProceedToNextDialogue(bool value)
    {
        NextDialogue = value;
    }


}

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
    private const float ANIMATION_DELAY = 0.05f;

    private Queue<TextAudioPath> m_storylineSentences;
    public TextMeshProUGUI myname;
    public Text maindialogue;
    public Animator myanimator;

    private bool NextDialogue { get; set; } = false;

    private VoiceId InterlocutorVoice { get; set; }

    [SerializeField]
    public AWSPollyDialogueTriggerEvent AWSPollyDialogueTriggerEvent;
    public NextDialogueTriggerEvent nextDialogueTriggerEvent;
    public DialogueTakingPlaceEvent dialogueTakingPlaceEvent;

    void Start()
    {
        m_storylineSentences = new Queue<TextAudioPath>();

        nextDialogueTriggerEvent.AddListener(ShouldProceedToNextDialogue);
        dialogueTakingPlaceEvent.AddListener(EndDialogue);
    }

    public async void PrepareDialoguesQueue(Dialogues dialogue)
    {
        m_storylineSentences.Clear();  //clears the previous dialogues, if there are any

        myname.text = dialogue.EntityName;

        foreach (TextAudioPath textAudioPath in dialogue.TextAudioPath)
        {
            m_storylineSentences.Enqueue(textAudioPath);
        }

        await PrepareInterlocutorVoice(dialogue);

    }

    private Task PrepareInterlocutorVoice(Dialogues dialogue)
    {
        //parses and assign the returned voiceID to the property inside the Dialogues class
        dialogue.ParseVoiceId();

        InterlocutorVoice = dialogue.VoiceID;

        return Task.CompletedTask;
    }

    private IEnumerator AnimateLetters(string sentence, float animationDelay)
    {
        maindialogue.text = string.Empty;

        for (int i = 0; i < sentence.Length; i++)
        {
            yield return new WaitForSeconds(animationDelay);
            maindialogue.text += sentence[i];
        }
    }

    public Task InvokeAIVoiceEvent(AWSPollyDialogueTriggerEvent awsPollyDialogueTriggerEvent, AWSPollyAudioPacket awsPollyAudioPacket)
    {
        awsPollyDialogueTriggerEvent.Invoke(awsPollyAudioPacket);

        return Task.CompletedTask;
    }

    public IEnumerator StartDialogue(SemaphoreSlim dialogueSemaphore)
    {

        if (m_storylineSentences.Count == 0) 
        {
            dialogueSemaphore.Release();

            yield return null;
        }
        else
        {
            NextDialogue = false;

            TextAudioPath dialogue = m_storylineSentences.Dequeue();

            InvokeAIVoiceEvent(AWSPollyDialogueTriggerEvent, new AWSPollyAudioPacket { DialogueText = dialogue.Sentence, AudioVoiceId = InterlocutorVoice, AudioName = dialogue.AudioPath });

            Coroutine animateLetter = StartCoroutine(AnimateLetters(dialogue.Sentence, ANIMATION_DELAY));

            yield return new WaitUntil(() => NextDialogue == true);

            StopCoroutine(animateLetter);

            StartCoroutine(StartDialogue(dialogueSemaphore));
        }
    }

    private void EndDialogue(bool dialogueTakingPlace)
    {
        myanimator.SetBool(DIALOGUE_ANIMATION_NAME, dialogueTakingPlace);
    }

    private void ShouldProceedToNextDialogue(bool value)
    {
        NextDialogue = value;
    }


}


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

    [SerializeField]
    public NextDialogueTriggerEvent nextDialogueTriggerEvent;
    public DialogueTakingPlaceEvent dialogueTakingPlaceEvent;
    public AudioTriggerEvent audioTriggerEvent;

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
            if  (textAudioPath.AudioPath == null || textAudioPath.AudioPath.Length == 0)
            {
                throw new ApplicationException($"AudioPath cannot be null!");
            }

            m_storylineSentences.Enqueue(textAudioPath);
        }
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

            Coroutine animateLetter = StartCoroutine(AnimateLetters(dialogue.Sentence, ANIMATION_DELAY));

            audioTriggerEvent.Invoke(new AudioPackage() { AudioName = dialogue.AudioName, AudioPath = dialogue.audioPath, AudioType = UnityEngine.AudioType.MPEG });

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

    private Task triggerAudio(Dialogues dialogues)
    {
        return Task.CompletedTask;
    }
}

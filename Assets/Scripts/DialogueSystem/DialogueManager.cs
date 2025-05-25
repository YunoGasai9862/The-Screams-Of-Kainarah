
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ObserverSystem(SubjectType = typeof(GlobalGameState), ObserverType = typeof(DialogueManager))]
public class DialogueManager : MonoBehaviour, IObserver<GameState> { 


    private const string DIALOGUE_ANIMATION_NAME = "IsOpen";
    private const float ANIMATION_DELAY = 0.05f;

    public TextMeshProUGUI myname;
    public Text maindialogue;
    public Animator myanimator;

    private bool NextDialogue { get; set; } = false;
    private DialoguePiece DialoguePiece { get; set; } = new DialoguePiece();

    [SerializeField]
    public NextDialogueTriggerEvent nextDialogueTriggerEvent;
    public AudioTriggerEvent audioTriggerEvent;

    void Start()
    {
        nextDialogueTriggerEvent.AddListener(ShouldProceedToNextDialogue);
    }

    public async void PrepareDialoguesQueue(DialogueSetup dialogueSetup)
    {
        DialoguePiece.DialogueQueue.Clear();  //clears the previous dialogues, if there are any

        myname.text = dialogueSetup.EntityName;

        foreach (Dialogue dialogue in dialogueSetup.Dialogues)
        {
            if  (dialogue.AudioInfo.AudioPath == null || dialogue.AudioInfo.AudioPath.Length == 0)
            {
                throw new ApplicationException($"AudioPath cannot be null!");
            }

            DialoguePiece.DialogueQueue.Enqueue(dialogue);
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
        if (DialoguePiece.DialogueQueue.Count == 0) 
        {
            dialogueSemaphore.Release();

            PingListeners(DialoguePiece.DialogueListeners, true);

            yield return null;
        }
        else
        {
            NextDialogue = false;

            Dialogue dialogue = DialoguePiece.DialogueQueue.Dequeue();

            Coroutine animateLetter = StartCoroutine(AnimateLetters(dialogue.Sentence, ANIMATION_DELAY));

            audioTriggerEvent.Invoke(new AudioPackage() { AudioName = dialogue.AudioInfo.AudioName, AudioPath = dialogue.AudioInfo.audioPath, AudioType = UnityEngine.AudioType.MPEG });

            yield return new WaitUntil(() => NextDialogue == true);

            StopCoroutine(animateLetter);

            StartCoroutine(StartDialogue(dialogueSemaphore));
        }
    }

    private void ShouldProceedToNextDialogue(bool value)
    {
        NextDialogue = value;
    }

    private void PingListeners(List<INotify<bool>> dialogueListeners, bool dialogueConcluded)
    {
        foreach(INotify<bool> listener in dialogueListeners)
        {
            listener.Notify(dialogueConcluded);
        }
    }

    public void OnNotify(GameState data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        //ill need to add this here because it was before in EndDialogue
        //myanimator.SetBool(DIALOGUE_ANIMATION_NAME, dialogueTakingPlace);
        throw new NotImplementedException();
    }
}

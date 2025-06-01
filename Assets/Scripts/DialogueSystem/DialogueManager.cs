
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour, IObserver<GameState>
{ 
    private const string DIALOGUE_ANIMATION_NAME = "IsOpen";
    private const float ANIMATION_DELAY = 0.05f;

    private bool NextDialogue { get; set; } = false;
    private DialoguePiece DialoguePiece { get; set; } = new DialoguePiece();

    [SerializeField]
    TextMeshProUGUI interlocutorTextArea;
    [SerializeField]
    Text dialogueTextArea;
    [SerializeField]
    Animator dialogueTextAreaAnimation;
    [SerializeField]
    NextDialogueTriggerEvent nextDialogueTriggerEvent;
    [SerializeField]
    AudioTriggerEvent audioTriggerEvent;
    [SerializeField]
    GlobalGameStateDelegator globalGameStateDelegator;

    void Start()
    {
        nextDialogueTriggerEvent.AddListener(ShouldProceedToNextDialogue);

        globalGameStateDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = this.name,
            ObserverTag = this.name,
            SubjectType = typeof(GlobalGameStateManager).ToString()

        }, CancellationToken.None);
    }

    public async void PrepareDialoguesQueue(DialogueSetup dialogueSetup)
    {
        DialoguePiece.DialogueQueue.Clear();  //clears the previous dialogues, if there are any

        interlocutorTextArea.text = dialogueSetup.EntityName;

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
        dialogueTextArea.text = string.Empty;

        for (int i = 0; i < sentence.Length; i++)
        {
            yield return new WaitForSeconds(animationDelay);

            dialogueTextArea.text += sentence[i];
        }
    }

    public IEnumerator StartDialogue(SemaphoreSlim dialogueSemaphore)
    {
        dialogueTextAreaAnimation.SetBool(DIALOGUE_ANIMATION_NAME, true);

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
        Debug.Log($"ShouldProceedToNextDialogue {value}");

        NextDialogue = value;
    }

    private void PingListeners(List<INotify<bool>> dialogueListeners, bool dialogueConcluded)
    {
        foreach(INotify<bool> listener in dialogueListeners)
        {
            Debug.Log("Here in Ping Listeners!");

            listener.Notify(dialogueConcluded);
        }
    }

    public void OnNotify(GameState data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        Debug.Log($"OnNotifyForDialogueManager {data}");

        //FIX THIS

        dialogueTextAreaAnimation.SetBool(DIALOGUE_ANIMATION_NAME, data.Equals(GameState.DIALOGUE_TAKING_PLACE) ? true: false);
    }
}

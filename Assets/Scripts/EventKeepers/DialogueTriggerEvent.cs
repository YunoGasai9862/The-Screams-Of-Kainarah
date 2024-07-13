using System.Threading.Tasks;
using UnityEngine.Events;

public class DialogueTriggerEvent : UnityEventWTAsync<DialoguesAndOptions.DialogueSystem>
{
    private UnityEvent<DialoguesAndOptions.DialogueSystem> m_dialogueTriggerEvent = new UnityEvent<DialoguesAndOptions.DialogueSystem>();
    public override Task AddListener(UnityAction<DialoguesAndOptions.DialogueSystem> action)
    {
        m_dialogueTriggerEvent.AddListener(action);

        return Task.CompletedTask;
    }

    public override UnityEvent<DialoguesAndOptions.DialogueSystem> GetInstance()
    {
        return m_dialogueTriggerEvent;
    }

    public override Task Invoke(DialoguesAndOptions.DialogueSystem value)
    {
        m_dialogueTriggerEvent.Invoke(value);

        return Task.CompletedTask;
    }
}
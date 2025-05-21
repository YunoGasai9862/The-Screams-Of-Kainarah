using NUnit.Framework;
using System.Collections.Generic;

public class DialogueBubble
{
    public Queue<TextAudioPath> TextAudioPaths { get; set; } = new Queue<TextAudioPath>();

    public List<DialogueSubscriberEntity> Subscribers { get; set; } = new List<DialogueSubscriberEntity>();
}
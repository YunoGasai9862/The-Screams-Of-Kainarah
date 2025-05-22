using NUnit.Framework;
using System.Collections.Generic;

public class DialoguePiece
{
    public Queue<Dialogue> Dialogue { get; set; } = new Queue<Dialogue>();
    public List<DialogueSubscriberEntity> Subscribers { get; set; } = new List<DialogueSubscriberEntity>();
}
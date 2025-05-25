using NUnit.Framework;
using System.Collections.Generic;

public class DialoguePiece
{
    public Queue<Dialogue> DialogueQueue { get; set; } = new Queue<Dialogue>();
    public List<INotify<bool>> DialogueListeners { get; set; } = new List<INotify<bool>>();
}
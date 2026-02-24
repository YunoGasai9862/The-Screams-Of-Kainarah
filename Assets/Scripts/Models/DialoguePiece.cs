using System.Collections.Generic;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;

public class DialoguePiece
{
    public Queue<Dialogue> DialogueQueue { get; set; } = new Queue<Dialogue>();
    public List<INotify<bool>> DialogueListeners { get; set; } = new List<INotify<bool>>();
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CheckPoints;
using static DialoguesAndOptions;

public class PlayerObserverListenerHelper //add new observers here
{
    private static ObserverList<Collider2D> colliderSubjects = new();

    private static ObserverList<bool> boolSubjects = new();

    private static ObserverList<DialogueSystem> _dialogueSystem = new();

    private static ObserverList<EntitiesToReset> _entitiesToReset = new();

    private static ObserverList<Checkpoint> _checkpoint = new ();

    private static ObserverList<GameObject> _mainPlayer = new();

    public static ObserverList<Collider2D> ColliderSubjects { get => colliderSubjects; }
    public static ObserverList<bool> BoolSubjects { get => boolSubjects; }
    public static ObserverList<DialogueSystem> DialogueSystem { get => _dialogueSystem; }
    public static ObserverList<EntitiesToReset> EntitiesToReset { get => _entitiesToReset; }
    public static ObserverList<Checkpoint> CheckPointsObserver {  get => _checkpoint; }
    public static ObserverList<GameObject> MainPlayerListener { get => _mainPlayer; }

}

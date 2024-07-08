using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CheckPoints;
using static DialoguesAndOptions;

public class PlayerObserverListenerHelper //add new observers here
{
    private static SubjectsToBeNotified<Collider2D> colliderSubjects = new();

    private static SubjectsToBeNotified<bool> boolSubjects = new();

    private static SubjectsToBeNotified<DialogueSystem> _dialogueSystem = new();

    private static SubjectsToBeNotified<EntitiesToReset> _entitiesToReset = new();

    private static SubjectsToBeNotified<Checkpoint> _checkpoint = new ();

    private static SubjectsToBeNotified<GameObject> _mainPlayer = new();

    private static SubjectsToBeNotified<DialoguesAndOptions> _dialogueAndOptions = new();

    public static SubjectsToBeNotified<Collider2D> ColliderSubjects { get => colliderSubjects; }
    public static SubjectsToBeNotified<bool> BoolSubjects { get => boolSubjects; }
    public static SubjectsToBeNotified<DialogueSystem> DialogueSystem { get => _dialogueSystem; }
    public static SubjectsToBeNotified<EntitiesToReset> EntitiesToReset { get => _entitiesToReset; }
    public static SubjectsToBeNotified<Checkpoint> CheckPointsObserver {  get => _checkpoint; }
    public static SubjectsToBeNotified<GameObject> MainPlayerListener { get => _mainPlayer; }
    public static SubjectsToBeNotified<DialoguesAndOptions> DialogueAndOptions { get => _dialogueAndOptions; }

}

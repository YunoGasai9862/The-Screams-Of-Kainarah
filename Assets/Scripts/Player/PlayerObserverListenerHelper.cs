using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CheckPoints;
using static DialogueEntityScriptableObject;

public class PlayerObserverListenerHelper //add new observers here
{
    private static SubjectsToBeNotified<Collider2D> colliderSubjects = new();

    private static SubjectsToBeNotified<bool> boolSubjects = new();

    private static SubjectsToBeNotified<DialoguesAndOptions> _dialoguesAndOptions = new();

    private static SubjectsToBeNotified<EntitiesToReset> _entitiesToReset = new();

    private static SubjectsToBeNotified<Checkpoint> _checkpoint = new ();

    private static SubjectsToBeNotified<GameObject> _mainPlayer = new();

    public static SubjectsToBeNotified<Collider2D> ColliderSubjects { get => colliderSubjects; }
    public static SubjectsToBeNotified<bool> BoolSubjects { get => boolSubjects; }
    public static SubjectsToBeNotified<DialoguesAndOptions> DialoguesAndOptions { get => _dialoguesAndOptions; }
    public static SubjectsToBeNotified<EntitiesToReset> EntitiesToReset { get => _entitiesToReset; }
    public static SubjectsToBeNotified<Checkpoint> CheckPointsObserver {  get => _checkpoint; }
    public static SubjectsToBeNotified<GameObject> MainPlayerListener { get => _mainPlayer; }

}

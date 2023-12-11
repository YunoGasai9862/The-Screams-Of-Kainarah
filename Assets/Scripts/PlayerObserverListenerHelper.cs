using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static DialogueEntityScriptableObject;

public class PlayerObserverListenerHelper //add new observers here
{
    private static SubjectsToBeNotified<Collider2D> colliderSubjects = new();

    private static SubjectsToBeNotified<bool> boolSubjects = new();

    private static SubjectsToBeNotified<DialogueEntity> _entities = new();

    private static SubjectsToBeNotified<EntitiesToReset> _entitiesToReset = new();

    public static SubjectsToBeNotified<Collider2D> ColliderSubjects { get => colliderSubjects; }
    public static SubjectsToBeNotified<bool> BoolSubjects { get => boolSubjects; }
    public static SubjectsToBeNotified<DialogueEntity> DialogueEntites { get => _entities; }
    public static SubjectsToBeNotified<EntitiesToReset> EntitiesToReset { get => _entitiesToReset; }

}

public class SceneSingletonSubjects
{
    private SubjectsToBeNotified<SceneSingleton> sceneSingletonSubjects = new SubjectsToBeNotified<SceneSingleton>();

    public SubjectsToBeNotified<SceneSingleton> SingletonSubjects { get => sceneSingletonSubjects; }
}
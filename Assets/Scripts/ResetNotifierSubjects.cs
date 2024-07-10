public class ResetNotifierSubjects
{
    private static SubjectsToBeNotified<DialoguesAndOptions> _dialogueAndOptions = new SubjectsToBeNotified<DialoguesAndOptions>();


    public static SubjectsToBeNotified<DialoguesAndOptions> DialogueAndOptions { get => _dialogueAndOptions; }

}
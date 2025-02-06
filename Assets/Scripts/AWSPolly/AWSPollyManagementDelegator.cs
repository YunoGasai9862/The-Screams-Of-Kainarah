public class AWSPollyManagementDelegator: BaseDelegator<IAWSPolly>
{
    private void OnEnable()
    {
        Subject = new Subject<IObserver<IAWSPolly>>();
    }
}
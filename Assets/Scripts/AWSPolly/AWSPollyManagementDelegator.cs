public class AWSPollyManagementDelegator: BaseDelegator<IAWSPolly>
{
    private void Start()
    {
        Subject = new Subject<IObserver<IAWSPolly>>();
    }
}
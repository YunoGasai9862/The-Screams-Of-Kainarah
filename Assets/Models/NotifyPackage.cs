public class NotifyPackage
{
    public string EntityNameToNotify { get; set; }

    public NotifierEntity NotifierEntity {get; set; }

    public override string ToString()
    {
        return $"Tag: {EntityNameToNotify}, NotifierEntity {NotifierEntity.ToString()}";
    }

}
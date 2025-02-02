public class AWSAccessResource
{
    public string AccessKey { get; set; }

    public string SecretAccessKey { get; set; }

    public AWSAccessResource(string accessKey, string secretAccessKey)
    {
        AccessKey = accessKey;
        SecretAccessKey = secretAccessKey;
    }

    public override string ToString()
    {
        return $"{AccessKey}:{SecretAccessKey}";
    }
} 
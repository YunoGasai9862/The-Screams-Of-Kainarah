using UnityEngine;

public class NotificationContext
{
    public string GameObjectName { get; set; }
    public string GameObjectTag { get; set; }

    //this is optional and should be avoided - but comparatively better in the cases where you need to constantly calculate distance with that object,
    //so again and again querying and finding in the scene and then doing distance is not resource efficient
    public GameObject GameObject {get; set; } = null;
}
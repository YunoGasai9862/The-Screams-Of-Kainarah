using Firebase;
using Firebase.Database;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseDatabaseManager : MonoBehaviour, IFirebaseDatabase
{
    //use this later if ever required to use Firebase Database for inventory/user logins (quickly prototyping it for now)
    private DatabaseReference FirebaseDatabaseReference { get; set; }
    private async void Start()
    {
        FirebaseDatabase database = FirebaseDatabase.GetInstance(FirebaseApp.DefaultInstance, "https://the-screams-of-kainarah-default-rtdb.europe-west1.firebasedatabase.app/");
        Debug.Log(database);
        FirebaseDatabaseReference = database.RootReference;

        //test to check
        await Create(new Player(SystemInfo.deviceUniqueIdentifier, "Muhammad Bilal", "TES"), new UsersNode());
    }

    public async Task Create(IEntity entity, INode firebaseNode)
    {
        //to generate unique UID - use SystemInfo.deviceUniqueIdentifier
        try
        {
            Debug.Log("Creating node for entity: " + entity.GetUID() + " in node: " + firebaseNode.GetNode());
            string json = JsonUtility.ToJson(entity.ToString());
            Debug.Log($"JSON: {json}");
            await FirebaseDatabaseReference.Child(firebaseNode.GetNode()).Child(entity.GetUID()).SetRawJsonValueAsync(json);
            Debug.Log("Node created successfully");
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }

    }

    public DatabaseReference GetDatabaseReference()
    {
        return FirebaseDatabaseReference;
    }

}
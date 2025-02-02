using Firebase;
using Firebase.Database;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

public class FirebaseDatabaseManager : MonoBehaviour, IFirebaseDatabase
{
    //use this later if ever required to use Firebase Database for inventory/user logins (quickly prototyping it for now)
    private DatabaseReference FirebaseDatabaseReference { get; set; }

    public async Task CreateEntity(IEntity entity, INode firebaseNode)
    {
        //to generate unique UID - use SystemInfo.deviceUniqueIdentifier
        try
        {
            Debug.Log("Creating node for entity: " + SystemInfo.deviceUniqueIdentifier + " in node: " + firebaseNode.GetNode());
            string json = JsonConvert.SerializeObject(entity);
            Debug.Log($"JSON: {json}");
            await FirebaseDatabaseReference.Child(firebaseNode.GetNode()).Child(SystemInfo.deviceUniqueIdentifier).SetRawJsonValueAsync(json);
            Debug.Log("Node created successfully");
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }

    }

    public Task SetupFirebaseDB()
    {
        FirebaseDatabase database = FirebaseDatabase.GetInstance(FirebaseApp.DefaultInstance, "https://the-screams-of-kainarah-default-rtdb.europe-west1.firebasedatabase.app/");
         Debug.Log(database);
        FirebaseDatabaseReference = database.RootReference;
        return Task.CompletedTask;
    }

    public DatabaseReference GetDatabaseReference()
    {
        return FirebaseDatabaseReference;
    }

    public Task UpdateEntity(IEntity entity, INode firebaseNode)
    {
        throw new NotImplementedException();
    }
}
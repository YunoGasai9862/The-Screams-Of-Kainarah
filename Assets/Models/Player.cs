using System;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class Player : IEntity
{
   public Player(string userUID, string username, string password)
    {
        UserUID = userUID;
        Username = username;
        Password = password;

        var value = JsonConvert.SerializeObject(this);
        Debug.Log(value);
    }

    public string UserUID { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public string GetUID()
    {
        return UserUID;
    }


}
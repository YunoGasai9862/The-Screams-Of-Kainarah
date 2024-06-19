using System;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class Player : IEntity
{
   public Player(string username, string password)
    {
        Username = username;
        Password = password;

        var value = JsonConvert.SerializeObject(this);
        Debug.Log(value);
    }

    public string Username { get; set; }
    public string Password { get; set; }

}
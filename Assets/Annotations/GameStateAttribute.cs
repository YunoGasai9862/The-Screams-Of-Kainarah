using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class GameStateAttribute: Attribute
{
    public Type EntityType { get; set; }

    public GameStateAttribute(Type entityType)
    {
        if (!typeof(IGameStateListener).IsAssignableFrom(entityType)) 
        {
            throw new ArgumentException("The given type must be an implementing type of IGameStateListener");
        }

       EntityType = entityType;
    }
}

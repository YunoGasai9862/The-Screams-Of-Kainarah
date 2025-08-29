using Pathfinding.Util;
using UnityEngine;
public class Player: IEntityTransform, IEntityHealth
{
   public Transform Transform { get; set; } 

   public Health Health { get; set; }
}
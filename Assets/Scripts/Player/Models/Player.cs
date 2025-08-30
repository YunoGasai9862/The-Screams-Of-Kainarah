using Pathfinding.Util;
using UnityEngine;
public class Player: IEntityTransform, IEntityHealth, IEntityRigidBody
{
   public Transform Transform { get; set; } 

   public Health Health { get; set; }

   public Rigidbody2D Rigidbody { get; set; }
}
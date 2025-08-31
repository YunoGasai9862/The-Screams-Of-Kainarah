using UnityEngine;
public class Player: IEntityTransform, IEntityHealth, IEntityRigidBody, IEntityAnimator, IEntitySpriteRenderer, IEntityCollider<Collider2D>
{
   public Transform Transform { get; set; } 

   public Health Health { get; set; }

   public Rigidbody2D Rigidbody { get; set; }

   public Collider2D Collider { get; set; }

   public SpriteRenderer Renderer { get; set; }

   public Animator Animator { get; set; }   
}
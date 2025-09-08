using UnityEngine;
public class Player: IEntityTransform, IEntityHealth, IEntityRigidBody, IEntityAnimator, IEntityCollider<Collider2D>
{
    public Transform Transform { get; set; } 

    public Health Health { get; set; }

    public Rigidbody2D Rigidbody { get; set; }

    public Collider2D Collider { get; set; }

    public SpriteRenderer SpriteRendererValue { get; set; }

    public Animator Animator { get; set; }

    public DefaultRenderer DefaultRendererValue { get; set; }

    public class SpriteRenderer : IEntityRenderer<UnityEngine.SpriteRenderer>
    {
        public UnityEngine.SpriteRenderer Renderer { get; set; }
    }

    public class DefaultRenderer : IEntityRenderer<UnityEngine.Renderer>
    {
        public UnityEngine.Renderer Renderer { get; set; }
    }
}
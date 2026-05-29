using Annotations.Enums;
using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV2;
using System.Collections.Generic;
using UnityEngine;

[Subject(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(PlayerAttributesNotifier), ContextType = typeof(Player))]
[Subject(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(PlayerAttributesNotifier), ContextType = typeof(IEntityTransform))]
public class PlayerAttributesNotifier: MonoBehaviour, IRequest<Player>, IRequest<IEntityAnimator>
{
    private Player Player { get; set; }

    private Health PlayerHealth { get; set; }

    private Delegator Delegator { get; set; }

    private async void OnEnable()
    {
        PlayerHealth = new Health()
        {
            MaxHealth = 100f,
            CurrentHealth = 100f,
            EntityName = name
        };

        Player = new Player() {

            Transform = GetComponent<Transform>(),
            Animator = GetComponent<Animator>(),
            Collider = GetComponent<CapsuleCollider2D>(),
            SpriteRendererValue = new Player.SpriteRenderer()
            {
                Renderer = GetComponent<SpriteRenderer>()
            },
            DefaultRendererValue = new Player.DefaultRenderer()
            { 
                Renderer = GetComponent<Renderer>()
            }, 
            Rigidbody = GetComponent<Rigidbody2D>(),
            Health = PlayerHealth,
        };

       StartCoroutine(Helper.GetDelegator<Delegator>(OnDelegatorFound));
    }

    IEnumerator<Player> IRequest<Player>.Request()
    {
        StartCoroutine(Delegator.NotifyObservers(new SubjectContext<Player> { EntityType = typeof(PlayerAttributesNotifier), Data = Player }, this));

        yield return null;
    }

    IEnumerator<IEntityAnimator> IRequest<IEntityAnimator>.Request()
    {
        StartCoroutine(Delegator.NotifyObservers(new SubjectContext<IEntityTransform> { EntityType = typeof(PlayerAttributesNotifier), Data = (IEntityTransform) Player.Transform }, (Assets.Scripts.Interfaces.Mediator.EnhancedV2.IRequest<IEntityTransform>) this));

        yield return null;
    }
}
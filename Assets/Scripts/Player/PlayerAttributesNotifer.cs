using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using System.Collections.Generic;
using UnityEngine;

[Subject(SubjectType = typeof(PlayerAttributesNotifier), ContextType = typeof(Player))]
public class PlayerAttributesNotifier: MonoBehaviour, IRequest<Player>
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

        Delegator = await Helper.GetDelegator<Delegator>();
    }

    public IEnumerator<Player> Request()
    {
        StartCoroutine(Delegator.NotifyObservers(new SubjectContext<Player> { EntityType = typeof(PlayerAttributesNotifier), Data = Player }, this));

        yield return null;
    }
}
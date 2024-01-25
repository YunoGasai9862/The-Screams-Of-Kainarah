using System;
using UnityEngine;
namespace PlayerHittableItemsNS
{
    [CreateAssetMenu(fileName = "PlayerHittableItems", menuName = "Player Hittable Items Object")]
    public class PlayerHittableItemsScriptableObject : ScriptableObject
    {
        [Serializable]
        public class playerHittableItems
        {
            public bool canHitPlayer;
            public Collider2D collider;
            public bool isItBasedOnAnimationName;
            [HideInInspector]
            public string animationName;
        }

        public playerHittableItems[] colliderItems;
    }

}

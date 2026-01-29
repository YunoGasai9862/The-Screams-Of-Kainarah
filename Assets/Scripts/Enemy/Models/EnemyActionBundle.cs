using UnityEngine;

namespace Assets.Scripts.Enemy.Models
{
    public class EnemyActionBundle
    {
        public Collider2D Target { get; set; }

        public string ActionName { get; set; }

        public object ActionValue { get; set; }
    }
}

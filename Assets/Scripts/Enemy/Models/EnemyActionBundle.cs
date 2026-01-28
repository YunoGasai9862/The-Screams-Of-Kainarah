using UnityEngine;

namespace Assets.Scripts.Enemy.Models
{
    public class EnemyActionBundle
    {
        private Collider2D Target { get; set; }

        private string ActionName { get; set; }

        private object ActionValue { get; set; }
    }
}

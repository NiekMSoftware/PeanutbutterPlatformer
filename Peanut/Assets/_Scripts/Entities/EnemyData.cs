using UnityEngine;

namespace _Scripts.Entities
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Entities/Enemies")]
    public class EnemyData : Entity
    {
        public override float DealDamage()
        {
            return base.DealDamage();
        }

        public override void TakeDamage(float damage)
        {
            Health -= damage;
        }

        public override bool HasDied()
        {
            return Health <= 0;
        }
    }
}
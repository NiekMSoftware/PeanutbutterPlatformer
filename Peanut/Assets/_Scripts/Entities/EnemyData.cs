using UnityEngine;

namespace _Scripts.Entities
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Entities/Enemies/Basic")]
    public class EnemyData : Entity
    {
        [Header("Enemy Data Properties")]
        [Range(1f, 20f)]
        public float Speed;
        
        public override float DealDamage()
        {
            float damage = Damage;

            return damage;
        }

        public override void TakeDamage(float damage)
        {
            Health -= damage;
        }

        public override void Attack()
        {
            Debug.Log("Commencing Attack Sequence");
        }

        public override bool HasDied() => Health <= 0;
    }
}
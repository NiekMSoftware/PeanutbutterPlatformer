using UnityEngine;

namespace _Scripts.Entities
{
    [CreateAssetMenu(fileName = "ScouterData", menuName = "Entities/Enemies/Scout")]
    public class ScoutData : EnemyData
    {
        public override void Attack()
        {
            
        }

        public override float DealDamage()
        {
            return 0f;
        }

        public override void TakeDamage(float damage)
        {
            
        }

        public override bool HasDied() => base.HasDied();
    }
}
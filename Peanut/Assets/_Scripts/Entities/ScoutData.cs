using UnityEngine;

namespace _Scripts.Entities
{
    [CreateAssetMenu(fileName = "ScouterData", menuName = "Entities/Enemies/Scout")]
    public class ScoutData : EnemyData
    {
        public override void Attack()
        {
            // play attack animation, animation event would deal damage.
            
            // check if player is still in range, if not find the player.
            // if that is unsuccessful, go back to patrolling
        }

        public override void TakeDamage(float damage)
        {
            
        }
    }
}
using Assets._Scripts.Interfaces;
using UnityEngine;

namespace Assets._Scripts.Entities
{
    /// <summary>
    /// Player is a child class of <c>Entity</c>, the player class has got all the accessible statistics a creature would need.
    /// </summary>
    [CreateAssetMenu(fileName = "Creature", menuName = "Entities/Creature", order = 1)]
    public class Player : Entity, ICreature
    {
        public void TakeDamage()
        {
            
        }

        public void Die()
        {
            Debug.Log("Player has died!");
        }
    }
}
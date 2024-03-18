using UnityEngine;

namespace _Scripts.Entities
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Entities/Player")]
    public class PlayerData : Entity
    {
        [Header("Movement Properties")]
        [Range(1f,  10f)]
        public float Speed;
        [Range(1f, 10f)]
        public float SprintSpeed;

        [Range(1f, 20f)]
        public float Drag;

        [Range(1f, 20f)]
        public float JumpForce;

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
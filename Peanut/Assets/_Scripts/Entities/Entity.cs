using UnityEngine;

namespace Assets._Scripts.Entities
{
    public abstract class Entity : ScriptableObject
    {
        [Header("Entity Data")] 
        [SerializeField] protected float maxHealth;
        protected float health;

        [SerializeField] protected bool isDead;

        [Header("Entity Movement Statistics")] 
        [SerializeField] protected float speed;
        [SerializeField] protected float jumpForce;
        [SerializeField] protected float dragAmount;

        #region Public Properties

        public float MaxHealth
        {
            get => maxHealth;
            set => maxHealth = value;
        }

        public float Health
        {
            get => health;
            set => health = value;
        }

        public float Speed
        {
            get => speed;
            set => speed = value;
        }

        public float JumpForce
        {
            get => jumpForce;
            set => jumpForce = value;
        }

        public bool IsDead
        {
            get => isDead;
            set => isDead = value;
        }

        public float DragAmount
        {
            get => dragAmount;
            set => dragAmount = value;
        }

        #endregion

    }
}

using Assets._Scripts.Interfaces;
using UnityEngine;

namespace _Scripts.Entities
{
    public abstract class Entity : ScriptableObject, IEntity
    {
        [Header("Base Entity Properties")]
        public float Health;
        public float MaxHealth;

        [Space(5)]
        public float Damage;

        [Header("Post-Processing")]
        public Animator EntityAnimator;

        [Space(5)]
        public AudioSource SfxSource;
        public AudioClip SfxClip;

        public virtual void TakeDamage(float damage)
        {
            throw new System.NotImplementedException();
        }

        public virtual float DealDamage()
        {
            throw new System.NotImplementedException();
        }

        public virtual bool HasDied()
        {
            throw new System.NotImplementedException();
        }
    }
}
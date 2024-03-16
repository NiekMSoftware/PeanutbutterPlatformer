using Assets._Scripts.Interfaces;
using UnityEngine;

namespace _Scripts.Entities
{
    public abstract class Entity : ScriptableObject, IEntity
    {
        [Header("Base Entity Properties")]
        public float MaxHealth;
        public float Health;

        [Space(5)]
        public float Damage;

        [Header("Post-Processing")]
        public Animator EntityAnimator;
        public ParticleSystem EntityParticles;

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

        public virtual void Attack()
        {
            throw new System.NotImplementedException();
        }

        public virtual bool HasDied()
        {
            throw new System.NotImplementedException();
        }
    }
}
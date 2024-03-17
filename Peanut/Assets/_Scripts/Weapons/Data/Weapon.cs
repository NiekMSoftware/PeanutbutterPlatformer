using UnityEngine;

namespace Assets._Scripts.Weapons
{
    public abstract class Weapon : ScriptableObject
    {
        [Header("Weapon Properties")] 
        [SerializeField] protected GameObject WeaponPrefab;

        [Space(10)]
        [SerializeField] protected string WeaponName;

        [Range(1f, 50f)]
        public float Damage;

        [Header("Post-Processing")] 
        public AudioSource WeaponSfx;
        public AudioClip WeaponSfxClip;
        
        [Space(5)]
        public ParticleSystem WeaponParticle;
        public Animator WeaponAnimator;
    }
}

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
        [SerializeField] protected float Damage;

        [Header("Post-Processing")] 
        [SerializeField] protected AudioSource WeaponSfx;
        [SerializeField] protected AudioClip WeaponSfxClip;
        
        [Space(5)]
        [SerializeField] protected ParticleSystem WeaponParticle;
        [SerializeField] protected Animator WeaponAnimator;
    }
}

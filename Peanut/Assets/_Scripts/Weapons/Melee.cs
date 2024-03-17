using Assets._Scripts.Interfaces;
using Assets._Scripts.Weapons;
using UnityEngine;

namespace _Scripts.Weapons
{
    public class Melee : MonoBehaviour, IWeapon
    {
        [Header("Melee Data")]
        public MeleeData MeleeData;

        public float DealDamage()
        {
            float damage = MeleeData.Damage;

            return damage;
        }
    }
}

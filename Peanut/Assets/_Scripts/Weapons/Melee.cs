using UnityEngine;

namespace Assets._Scripts.Weapons
{
    [CreateAssetMenu(fileName = "Melee", menuName = "Weapons/Melee", order = 2)]
    public class Melee : Weapon
    {
        [Header("Melee Properties")]
        [Range(0.1f, 5f)]
        [SerializeField] private float meleeSpeed;
    }
}

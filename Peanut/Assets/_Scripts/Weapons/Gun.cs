using UnityEngine;

namespace Assets._Scripts.Weapons
{
    [CreateAssetMenu(fileName = "Gun", menuName = "Weapons/Gun", order = 1)]
    public class Gun : Weapon
    {
        [Header("Gun Properties")] 
        [SerializeField] private GameObject bulletPrefab;

        [Space(10)]
        [SerializeField] private int magSize;
        [SerializeField] private float fireRate;

    }
}
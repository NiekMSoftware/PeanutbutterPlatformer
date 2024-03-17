using UnityEngine;

namespace Assets._Scripts.Weapons
{
    [CreateAssetMenu(fileName = "Gun", menuName = "Weapons/Gun", order = 1)]
    public class GunData : Weapon
    {
        [Header("Gun Properties")] 
        [SerializeField] private GameObject bulletPrefab;

        [Space(10)]
        [SerializeField] private int magSize;
        [SerializeField] private int currentMagSize;

        [Space(5)]
        [SerializeField] private float fireRate;
        [SerializeField] private float reloadTime;
    }
}
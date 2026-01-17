using UnityEngine;

[CreateAssetMenu(fileName = "new Weapon Class", menuName = "Item/Weapon")]
public class WeaponClass : ItemClass
{
    [Header("Weapon")]
    public GameObject weaponPrefab;

    public WeaponType weaponType;
    public enum WeaponType
    {
        primary,
        secondary
    }

    public override WeaponClass GetWeapon() { return this; }
}
